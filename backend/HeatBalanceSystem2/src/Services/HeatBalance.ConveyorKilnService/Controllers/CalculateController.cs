using HeatBalance.Contracts;
using HeatBalance.Math;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeatBalance.ConveyorKilnService.Controllers;

[ApiController]
[Route("api/v1")]
[Authorize(Policy = AuthPolicies.ConveyorCalculate)]
public sealed class CalculateController : ControllerBase
{
    [HttpPost("calculate")]
    public ActionResult<HeatBalanceResult> Calculate([FromBody] ConveyorKilnInput input)
    {
        try
        {
            return HeatBalanceCalculations.Calculate(input);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
