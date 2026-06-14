using System.ComponentModel.DataAnnotations;
using HeatBalance.Math;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductWebApp.Data;
using ProductWebApp.Services;
using ProductWebApp.Services.Backend;
using ProductWebApp.Utils;

namespace ProductWebApp.Pages.Cabinet;

public class CreateElectricKilnModel : PageModel
{
    private readonly IHeatBalanceBackend _backend;
    private readonly HeatBalanceRunService _runService;

    public CreateElectricKilnModel(IHeatBalanceBackend backend, HeatBalanceRunService runService)
    {
        _backend = backend;
        _runService = runService;
    }

    [BindProperty, Required, StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [BindProperty, StringLength(4000)]
    public string Comment { get; set; } = string.Empty;

    [BindProperty]
    public ElectricKilnInput Input { get; set; } = new();

    public void OnGet() => Input.HeatLossFraction = 0.1;

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        var userId = UserClaimsHelper.GetUserId(User)!;
        await _backend.CreateDataSetAsync(userId, new InputDataSet
        {
            Name = Name,
            Comment = Comment,
            CalculationType = CalculationType.ElectricKiln,
            InputJson = _runService.SerializeInput(CalculationType.ElectricKiln, Input)
        });
        return RedirectToPage("./Index");
    }
}
