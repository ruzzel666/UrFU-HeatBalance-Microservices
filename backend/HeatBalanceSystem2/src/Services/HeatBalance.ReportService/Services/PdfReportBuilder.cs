using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using HeatBalance.Contracts;
using HeatBalance.Math;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HeatBalance.ReportService.Services;

public sealed class PdfReportBuilder
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public byte[] Build(DatasetDto dataset, CalculationRunDto run)
    {
        var result = JsonSerializer.Deserialize<HeatBalanceResult>(run.ResultJson, JsonOptions)
                     ?? throw new InvalidOperationException("Не удалось прочитать результат расчёта.");

        QuestPDF.Settings.License = LicenseType.Community;

        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Column(col =>
                {
                    col.Item().Text("Отчёт по расчёту теплового баланса").SemiBold().FontSize(16);
                    col.Item().Text(dataset.CalculationType.ToDisplayNameRu()).FontSize(12);
                });

                page.Content().Column(col =>
                {
                    col.Spacing(10);
                    col.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(meta =>
                    {
                        meta.Spacing(4);
                        meta.Item().Text($"Набор данных: {dataset.Name}").SemiBold();
                        if (!string.IsNullOrWhiteSpace(dataset.Comment))
                            meta.Item().Text($"Комментарий: {dataset.Comment}");
                        meta.Item().Text($"Время расчёта (UTC): {run.ExecutedAt:yyyy-MM-dd HH:mm:ss}");
                    });

                    col.Item().Text("Итоги").SemiBold().FontSize(13);
                    col.Item().Table(t =>
                    {
                        t.ColumnsDefinition(c => { c.RelativeColumn(); c.ConstantColumn(140); });
                        AddRow(t, "Подвод энергии, кДж/ч", result.TotalInputKJPerHour);
                        AddRow(t, "Полезная теплота, кДж/ч", result.UsefulHeatKJPerHour);
                        AddRow(t, "Потери, кДж/ч", result.LossesKJPerHour);
                        AddRow(t, "Невязка/прочее, кДж/ч", result.UnaccountedKJPerHour);
                        t.Cell().ColumnSpan(2).PaddingTop(4).Text($"КПД: {(result.Efficiency * 100):F1}%");
                    });

                    if (!string.IsNullOrWhiteSpace(result.Notes))
                        col.Item().PaddingTop(4).Text(result.Notes).FontColor(Colors.Red.Darken2);
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Сформировано: ");
                    x.Span(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")).SemiBold();
                });
            });
        });

        return doc.GeneratePdf();
    }

    private static void AddRow(TableDescriptor t, string label, double value)
    {
        t.Cell().Padding(2).Text(label);
        t.Cell().Padding(2).AlignRight().Text($"{value:F2}");
    }
}

public sealed class RunApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public RunApiClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<CalculationRunDto> GetRunAsync(Guid runId, string userAccessToken, CancellationToken ct = default)
    {
        var baseUrl = _configuration["Services:Runs"] ?? "http://localhost:5200";
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userAccessToken);
        return await client.GetFromJsonAsync<CalculationRunDto>($"{baseUrl.TrimEnd('/')}/api/v1/runs/{runId}", ct)
               ?? throw new InvalidOperationException("Run not found.");
    }
}

public sealed class DatasetApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public DatasetApiClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<DatasetDto> GetDatasetAsync(Guid datasetId, string userAccessToken, CancellationToken ct = default)
    {
        var baseUrl = _configuration["Services:Dataset"] ?? "http://localhost:5100";
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userAccessToken);
        return await client.GetFromJsonAsync<DatasetDto>($"{baseUrl.TrimEnd('/')}/api/v1/datasets/{datasetId}", ct)
               ?? throw new InvalidOperationException("Dataset not found.");
    }
}
