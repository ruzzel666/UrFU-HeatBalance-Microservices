using HeatBalance.Contracts;
using HeatBalance.Math;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeatBalance.ChamberKilnService.Controllers;

[ApiController]
[Route("api/v1")]
[Authorize(Policy = AuthPolicies.ChamberCalculate)]
public sealed class CalculateController : ControllerBase
{
    [HttpPost("calculate")]
    public ActionResult<HeatBalanceResult> Calculate([FromBody] ChamberKilnInput input)
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
