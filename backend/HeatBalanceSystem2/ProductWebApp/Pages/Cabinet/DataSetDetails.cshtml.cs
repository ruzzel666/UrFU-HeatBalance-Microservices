using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductWebApp.Data;
using ProductWebApp.Services.Backend;
using ProductWebApp.Utils;

namespace ProductWebApp.Pages.Cabinet;

public class DataSetDetailsModel : PageModel
{
    private readonly IHeatBalanceBackend _backend;

    public DataSetDetailsModel(IHeatBalanceBackend backend) => _backend = backend;

    public InputDataSet DataSet { get; private set; } = default!;
    public List<CalculationRun> Runs { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var userId = UserClaimsHelper.GetUserId(User)!;
        var ds = await _backend.GetDataSetAsync(id, userId);
        if (ds is null) return NotFound();

        DataSet = ds;
        Runs = await _backend.GetRunsForDatasetAsync(id, userId);
        return Page();
    }

    public async Task<IActionResult> OnPostRunAsync(Guid id)
    {
        var userId = UserClaimsHelper.GetUserId(User)!;
        var runId = await _backend.RunCalculationAsync(id, userId);
        return RedirectToPage("./RunDetails", new { id = runId });
    }
}
