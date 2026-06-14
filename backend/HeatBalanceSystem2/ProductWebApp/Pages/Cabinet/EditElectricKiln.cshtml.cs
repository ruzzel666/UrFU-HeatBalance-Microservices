using System.ComponentModel.DataAnnotations;
using HeatBalance.Math;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductWebApp.Data;
using ProductWebApp.Services;
using ProductWebApp.Services.Backend;
using ProductWebApp.Utils;

namespace ProductWebApp.Pages.Cabinet;

public class EditElectricKilnModel : PageModel
{
    private readonly IHeatBalanceBackend _backend;
    private readonly HeatBalanceRunService _runService;

    public EditElectricKilnModel(IHeatBalanceBackend backend, HeatBalanceRunService runService)
    {
        _backend = backend;
        _runService = runService;
    }

    [BindProperty(SupportsGet = true)] public Guid Id { get; set; }
    [BindProperty, Required, StringLength(200)] public string Name { get; set; } = string.Empty;
    [BindProperty, StringLength(4000)] public string Comment { get; set; } = string.Empty;
    [BindProperty] public ElectricKilnInput Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = UserClaimsHelper.GetUserId(User)!;
        var ds = await _backend.GetDataSetAsync(Id, userId);
        if (ds is null) return NotFound();
        if (ds.CalculationType != CalculationType.ElectricKiln) return RedirectToPage("./Edit", new { id = Id });
        Name = ds.Name; Comment = ds.Comment; Input = (ElectricKilnInput)_runService.DeserializeInput(ds);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        var userId = UserClaimsHelper.GetUserId(User)!;
        var ds = await _backend.GetDataSetAsync(Id, userId);
        if (ds is null) return NotFound();
        if (ds.CalculationType != CalculationType.ElectricKiln) return RedirectToPage("./Edit", new { id = Id });
        ds.Name = Name; ds.Comment = Comment;
        ds.InputJson = _runService.SerializeInput(CalculationType.ElectricKiln, Input);
        await _backend.UpdateDataSetAsync(userId, ds);
        return RedirectToPage("./Index");
    }
}
