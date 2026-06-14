using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductWebApp.Data;
using ProductWebApp.Services.Backend;
using ProductWebApp.Utils;

namespace ProductWebApp.Pages.Admin;

public class DataSetsModel : PageModel
{
    private readonly IHeatBalanceBackend _backend;

    public DataSetsModel(IHeatBalanceBackend backend) => _backend = backend;

    public List<InputDataSet> DataSets { get; private set; } = new();

    public async Task OnGetAsync()
        => DataSets = await _backend.GetAllDataSetsAsync();

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        await _backend.DeleteDataSetAsAdminAsync(id);
        return RedirectToPage();
    }
}
