using HeatBalance.Contracts;
using HeatBalance.ReportService.Services;
using HeatBalance.ServiceDefaults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeatBalance.ReportService.Controllers;

[ApiController]
[Route("api/v1/reports")]
public sealed class ReportsController : ControllerBase
{
    private readonly PdfReportBuilder _pdfBuilder;
    private readonly RunApiClient _runClient;
    private readonly DatasetApiClient _datasetClient;

    public ReportsController(PdfReportBuilder pdfBuilder, RunApiClient runClient, DatasetApiClient datasetClient)
    {
        _pdfBuilder = pdfBuilder;
        _runClient = runClient;
        _datasetClient = datasetClient;
    }

    [HttpGet("{runId:guid}/pdf")]
    [Authorize(Policy = AuthPolicies.ReportsGenerate)]
    public async Task<IActionResult> GetPdf(Guid runId, CancellationToken ct)
    {
        var token = GetBearerToken();
        if (token is null) return Unauthorized();

        var run = await _runClient.GetRunAsync(runId, token, ct);
        var dataset = await _datasetClient.GetDatasetAsync(run.InputDataSetId, token, ct);
        var pdf = _pdfBuilder.Build(dataset, run);
        return File(pdf, "application/pdf", $"heat-balance-{runId:N}.pdf");
    }

    private string? GetBearerToken()
    {
        var header = Request.Headers.Authorization.ToString();
        const string prefix = "Bearer ";
        return header.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
            ? header[prefix.Length..].Trim()
            : null;
    }
}
