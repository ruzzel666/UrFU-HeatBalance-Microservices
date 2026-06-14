using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductWebApp.Data;
using ProductWebApp.Services.Backend;
using ProductWebApp.Utils;

namespace ProductWebApp.Pages.Cabinet;

public class IndexModel : PageModel
{
    private readonly IHeatBalanceBackend _backend;

    public IndexModel(IHeatBalanceBackend backend) => _backend = backend;

    public List<InputDataSet> DataSets { get; private set; } = new();

    public async Task OnGetAsync()
    {
        var userId = UserClaimsHelper.GetUserId(User)!;
        DataSets = await _backend.GetDataSetsAsync(userId);
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        var userId = UserClaimsHelper.GetUserId(User)!;
        await _backend.DeleteDataSetAsync(id, userId);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRunAsync(Guid id)
    {
        var userId = UserClaimsHelper.GetUserId(User)!;
        var runId = await _backend.RunCalculationAsync(id, userId);
        return RedirectToPage("./RunDetails", new { id = runId });
    }
}
