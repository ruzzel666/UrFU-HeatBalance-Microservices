using HeatBalance.Math;
using ProductWebApp.Data;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ProductWebApp.Services;

public sealed class PdfReportService
{
    public byte[] BuildRunReportPdf(InputDataSet dataSet, CalculationRun run, HeatBalanceResult result)
    {
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
                    col.Item().Text($"{GetTypeRu(dataSet.CalculationType)}").FontSize(12);
                });

                page.Content().Column(col =>
                {
                    col.Spacing(10);

                    col.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(meta =>
                    {
                        meta.Spacing(4);
                        meta.Item().Text($"Набор данных: {dataSet.Name}").SemiBold();
                        if (!string.IsNullOrWhiteSpace(dataSet.Comment))
                            meta.Item().Text($"Комментарий: {dataSet.Comment}");
                        meta.Item().Text($"Время расчёта (UTC): {run.ExecutedAt:yyyy-MM-dd HH:mm:ss}");
                    });

                    col.Item().Text("Итоги").SemiBold().FontSize(13);
                    col.Item().Table(t =>
                    {
                        t.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn();
                            c.ConstantColumn(140);
                        });

                        AddRow(t, "Подвод энергии, кДж/ч", result.TotalInputKJPerHour);
                        AddRow(t, "Полезная теплота, кДж/ч", result.UsefulHeatKJPerHour);
                        AddRow(t, "Потери, кДж/ч", result.LossesKJPerHour);
                        AddRow(t, "Невязка/прочее, кДж/ч", result.UnaccountedKJPerHour);
                        t.Cell().ColumnSpan(2).PaddingTop(4).Text($"КПД: {(result.Efficiency * 100):F1}%");
                    });

                    if (!string.IsNullOrWhiteSpace(result.Notes))
                    {
                        col.Item().PaddingTop(4).Text(result.Notes).FontColor(Colors.Red.Darken2);
                    }

                    col.Item().PaddingTop(8).Text("Статьи теплового баланса").SemiBold().FontSize(13);
                    col.Item().Table(t =>
                    {
                        t.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn();
                            c.ConstantColumn(140);
                        });

                        t.Header(h =>
                        {
                            h.Cell().Padding(4).Background(Colors.Grey.Lighten3).Text("Статья").SemiBold();
                            h.Cell().Padding(4).Background(Colors.Grey.Lighten3).AlignRight().Text("кДж/ч").SemiBold();
                        });

                        foreach (var c in result.Components)
                        {
                            t.Cell().Padding(4).Text(c.Name);
                            t.Cell().Padding(4).AlignRight().Text($"{c.ValueKJPerHour:F2}");
                        }
                    });
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

    private static string GetTypeRu(CalculationType type) => type switch
    {
        CalculationType.ConveyorKiln => "Конвейерная сушильная печь",
        CalculationType.ChamberKiln => "Камерная сушильная печь",
        CalculationType.ElectricKiln => "Электрическая сушильная печь",
        CalculationType.DrumDryer => "Сушильный барабан",
        _ => type.ToString()
    };
}

