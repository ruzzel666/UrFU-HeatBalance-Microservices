using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductWebApp.Data;
using ProductWebApp.Services.Backend;
using ProductWebApp.Utils;

namespace ProductWebApp.Pages.Cabinet;

public class EditModel : PageModel
{
    private readonly IHeatBalanceBackend _backend;

    public EditModel(IHeatBalanceBackend backend) => _backend = backend;

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var userId = UserClaimsHelper.GetUserId(User)!;
        var ds = await _backend.GetDataSetAsync(id, userId);
        if (ds is null) return NotFound();

        return ds.CalculationType switch
        {
            CalculationType.ConveyorKiln => RedirectToPage("./EditConveyorKiln", new { id }),
            CalculationType.ChamberKiln => RedirectToPage("./EditChamberKiln", new { id }),
            CalculationType.ElectricKiln => RedirectToPage("./EditElectricKiln", new { id }),
            CalculationType.DrumDryer => RedirectToPage("./EditDrumDryer", new { id }),
            _ => NotFound()
        };
    }
}
