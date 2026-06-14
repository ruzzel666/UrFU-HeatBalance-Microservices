using System.Security.Claims;
using HeatBalance.Contracts;
using HeatBalance.DatasetService.Data;
using HeatBalance.ServiceDefaults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeatBalance.DatasetService.Controllers;

[ApiController]
[Route("api/v1/datasets")]
public sealed class DatasetsController : ControllerBase
{
    private readonly DatasetDbContext _db;

    public DatasetsController(DatasetDbContext db) => _db = db;

    [HttpGet]
    [Authorize(Policy = AuthPolicies.DatasetsRead)]
    public async Task<ActionResult<List<DatasetDto>>> GetMine(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var query = _db.InputDataSets.AsQueryable();
        if (!User.IsInRole(AuthConstants.AdminRole) && !User.HasScope(AuthPolicies.Admin))
            query = query.Where(x => x.OwnerId == userId);

        var items = await query.ToListAsync(ct);
        return items
            .OrderByDescending(x => x.UpdatedAt)
            .Select(x => x.ToDto())
            .ToList();
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = AuthPolicies.DatasetsRead)]
    public async Task<ActionResult<DatasetDto>> GetById(Guid id, CancellationToken ct)
    {
        var entity = await _db.InputDataSets.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return NotFound();
        if (!CanAccess(entity)) return Forbid();
        return entity.ToDto();
    }

    [HttpPost]
    [Authorize(Policy = AuthPolicies.DatasetsWrite)]
    public async Task<ActionResult<DatasetDto>> Create([FromBody] CreateDatasetRequest request, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var entity = new InputDataSetEntity
        {
            OwnerId = userId,
            Name = request.Name,
            CalculationType = request.CalculationType,
            Comment = request.Comment,
            InputJson = request.InputJson
        };

        _db.InputDataSets.Add(entity);
        await _db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity.ToDto());
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = AuthPolicies.DatasetsWrite)]
    public async Task<ActionResult<DatasetDto>> Update(Guid id, [FromBody] UpdateDatasetRequest request, CancellationToken ct)
    {
        var entity = await _db.InputDataSets.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return NotFound();
        if (!CanAccess(entity)) return Forbid();

        entity.Name = request.Name;
        entity.Comment = request.Comment;
        entity.InputJson = request.InputJson;
        await _db.SaveChangesAsync(ct);
        return entity.ToDto();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = AuthPolicies.DatasetsDelete)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var entity = await _db.InputDataSets.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return NotFound();
        if (!CanAccess(entity)) return Forbid();

        _db.InputDataSets.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    private bool CanAccess(InputDataSetEntity entity)
    {
        if (User.IsInRole(AuthConstants.AdminRole) || User.HasScope(AuthPolicies.Admin))
            return true;

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return entity.OwnerId == userId;
    }
}
