using System.Security.Claims;
using System.Text.Json;
using HeatBalance.Contracts;
using HeatBalance.RunService.Data;
using HeatBalance.RunService.Services;
using HeatBalance.ServiceDefaults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeatBalance.RunService.Controllers;

[ApiController]
[Route("api/v1/runs")]
public sealed class RunsController : ControllerBase
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = false };

    private readonly RunDbContext _db;
    private readonly DatasetApiClient _datasetClient;
    private readonly FurnaceCalculationClient _furnaceClient;

    public RunsController(RunDbContext db, DatasetApiClient datasetClient, FurnaceCalculationClient furnaceClient)
    {
        _db = db;
        _datasetClient = datasetClient;
        _furnaceClient = furnaceClient;
    }

    [HttpPost]
    [Authorize(Policy = AuthPolicies.RunsExecute)]
    public async Task<ActionResult<CreateRunResponse>> Create([FromBody] CreateRunRequest request, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var userToken = GetBearerToken();
        if (userToken is null)
            return Unauthorized();

        DatasetDto dataset;
        try
        {
            dataset = await _datasetClient.GetDatasetAsync(request.InputDataSetId, userToken, ct);
        }
        catch (Exception ex)
        {
            return NotFound(new { error = ex.Message });
        }

        if (!CanAccessDataset(dataset, userId))
            return Forbid();

        var result = await _furnaceClient.CalculateAsync(dataset, ct);
        var run = new CalculationRunEntity
        {
            InputDataSetId = dataset.Id,
            OwnerId = dataset.OwnerId,
            ResultJson = JsonSerializer.Serialize(result, JsonOptions),
            ExecutedAt = DateTimeOffset.UtcNow
        };

        _db.CalculationRuns.Add(run);
        await _db.SaveChangesAsync(ct);

        return Ok(new CreateRunResponse
        {
            RunId = run.Id,
            ResultJson = run.ResultJson
        });
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = AuthPolicies.RunsRead)]
    public async Task<ActionResult<CalculationRunDto>> GetById(Guid id, CancellationToken ct)
    {
        var run = await _db.CalculationRuns.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (run is null) return NotFound();
        if (!CanAccessRun(run)) return Forbid();

        return new CalculationRunDto
        {
            Id = run.Id,
            InputDataSetId = run.InputDataSetId,
            ExecutedAt = run.ExecutedAt,
            ResultJson = run.ResultJson,
            Notes = run.Notes
        };
    }

    [HttpGet("by-dataset/{datasetId:guid}")]
    [Authorize(Policy = AuthPolicies.RunsRead)]
    public async Task<ActionResult<List<CalculationRunDto>>> GetByDataset(Guid datasetId, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        var query = _db.CalculationRuns.Where(x => x.InputDataSetId == datasetId);

        if (!User.IsInRole(AuthConstants.AdminRole) && !User.HasScope(AuthPolicies.Admin))
            query = query.Where(x => x.OwnerId == userId);

        var items = await query.ToListAsync(ct);
        return items
            .OrderByDescending(x => x.ExecutedAt)
            .Select(x => new CalculationRunDto
            {
                Id = x.Id,
                InputDataSetId = x.InputDataSetId,
                ExecutedAt = x.ExecutedAt,
                ResultJson = x.ResultJson,
                Notes = x.Notes
            })
            .ToList();
    }

    private bool CanAccessRun(CalculationRunEntity run)
    {
        if (User.IsInRole(AuthConstants.AdminRole) || User.HasScope(AuthPolicies.Admin))
            return true;

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return run.OwnerId == userId;
    }

    private bool CanAccessDataset(DatasetDto dataset, string userId)
    {
        if (User.IsInRole(AuthConstants.AdminRole) || User.HasScope(AuthPolicies.Admin))
            return true;

        return dataset.OwnerId == userId;
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
