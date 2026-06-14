using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductWebApp.Data;
using ProductWebApp.Services;
using ProductWebApp.Services.Backend;
using ProductWebApp.Utils;

namespace ProductWebApp.Pages.Cabinet;

public class RunDetailsModel : PageModel
{
    private readonly IHeatBalanceBackend _backend;
    private readonly HeatBalanceRunService _runService;
    private readonly PdfReportService _pdf;

    public RunDetailsModel(IHeatBalanceBackend backend, HeatBalanceRunService runService, PdfReportService pdf)
    {
        _backend = backend;
        _runService = runService;
        _pdf = pdf;
    }

    public CalculationRun Run { get; private set; } = default!;
    public InputDataSet DataSet { get; private set; } = default!;
    public HeatBalance.Math.HeatBalanceResult Result { get; private set; } = default!;

    public string ChartLabelsJson { get; private set; } = "[]";
    public string ChartValuesJson { get; private set; } = "[]";

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var userId = UserClaimsHelper.GetUserId(User)!;
        var run = await _backend.GetRunAsync(id, userId);
        if (run?.InputDataSet is null && run is not null)
        {
            var ds = await _backend.GetDataSetAsync(run.InputDataSetId, userId);
            if (ds is null && !UserClaimsHelper.IsAdmin(User)) return Forbid();
            if (ds is not null) run.InputDataSet = ds;
        }

        if (run?.InputDataSet is null) return NotFound();
        if (!CanAccess(run.InputDataSet)) return Forbid();

        Run = run;
        DataSet = run.InputDataSet;
        Result = _runService.DeserializeResult(run.ResultJson);

        ChartLabelsJson = JsonSerializer.Serialize(Result.Components.Select(c => c.Name).ToArray());
        ChartValuesJson = JsonSerializer.Serialize(Result.Components.Select(c => c.ValueKJPerHour).ToArray());
        return Page();
    }

    public async Task<IActionResult> OnGetPdfAsync(Guid id)
    {
        var userId = UserClaimsHelper.GetUserId(User)!;
        var run = await _backend.GetRunAsync(id, userId);
        if (run?.InputDataSet is null) return NotFound();
        if (!CanAccess(run.InputDataSet)) return Forbid();

        var remotePdf = await _backend.GetRunPdfAsync(id, userId);
        byte[] bytes;
        if (remotePdf is not null)
        {
            bytes = remotePdf;
        }
        else
        {
            var result = _runService.DeserializeResult(run.ResultJson);
            bytes = _pdf.BuildRunReportPdf(run.InputDataSet, run, result);
        }

        var safeName = string.Concat((run.InputDataSet.Name ?? "report").Select(ch => char.IsLetterOrDigit(ch) ? ch : '_'));
        return File(bytes, "application/pdf", $"{safeName}_{run.ExecutedAt:yyyyMMdd_HHmm}.pdf");
    }

    private bool CanAccess(InputDataSet ds)
    {
        if (UserClaimsHelper.IsAdmin(User)) return true;
        var userId = UserClaimsHelper.GetUserId(User)!;
        return ds.OwnerId == userId;
    }
}
