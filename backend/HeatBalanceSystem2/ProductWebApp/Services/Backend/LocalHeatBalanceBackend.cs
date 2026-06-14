using Microsoft.EntityFrameworkCore;
using ProductWebApp.Data;
using ProductWebApp.Services;

namespace ProductWebApp.Services.Backend;

public sealed class LocalHeatBalanceBackend : IHeatBalanceBackend
{
    private readonly ApplicationDbContext _db;
    private readonly HeatBalanceRunService _runService;

    public LocalHeatBalanceBackend(ApplicationDbContext db, HeatBalanceRunService runService)
    {
        _db = db;
        _runService = runService;
    }

    public async Task<List<InputDataSet>> GetDataSetsAsync(string userId, CancellationToken ct = default)
    {
        var items = await _db.InputDataSets.Where(x => x.OwnerId == userId).ToListAsync(ct);
        return items.OrderByDescending(x => x.UpdatedAt).ToList();
    }

    public async Task<List<InputDataSet>> GetAllDataSetsAsync(CancellationToken ct = default)
    {
        var items = await _db.InputDataSets.ToListAsync(ct);
        return items.OrderByDescending(x => x.UpdatedAt).ToList();
    }

    public Task<InputDataSet?> GetDataSetAsync(Guid id, string userId, CancellationToken ct = default)
        => _db.InputDataSets.FirstOrDefaultAsync(x => x.Id == id && x.OwnerId == userId, ct);

    public async Task<InputDataSet> CreateDataSetAsync(string userId, InputDataSet dataSet, CancellationToken ct = default)
    {
        dataSet.OwnerId = userId;
        _db.InputDataSets.Add(dataSet);
        await _db.SaveChangesAsync(ct);
        return dataSet;
    }

    public async Task UpdateDataSetAsync(string userId, InputDataSet dataSet, CancellationToken ct = default)
    {
        var existing = await _db.InputDataSets.FirstOrDefaultAsync(x => x.Id == dataSet.Id && x.OwnerId == userId, ct)
                       ?? throw new InvalidOperationException("Dataset not found.");
        existing.Name = dataSet.Name;
        existing.Comment = dataSet.Comment;
        existing.InputJson = dataSet.InputJson;
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteDataSetAsync(Guid id, string userId, CancellationToken ct = default)
    {
        var ds = await _db.InputDataSets.FirstOrDefaultAsync(x => x.Id == id && x.OwnerId == userId, ct);
        if (ds is null) return;
        _db.InputDataSets.Remove(ds);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteDataSetAsAdminAsync(Guid id, CancellationToken ct = default)
    {
        var ds = await _db.InputDataSets.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (ds is null) return;
        _db.InputDataSets.Remove(ds);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<Guid> RunCalculationAsync(Guid datasetId, string userId, CancellationToken ct = default)
    {
        var ds = await _db.InputDataSets.FirstOrDefaultAsync(x => x.Id == datasetId && x.OwnerId == userId, ct)
                 ?? throw new InvalidOperationException("Dataset not found.");
        var result = _runService.Calculate(ds);
        var run = new CalculationRun
        {
            InputDataSetId = ds.Id,
            ResultJson = _runService.SerializeResult(result),
            ExecutedAt = DateTimeOffset.UtcNow
        };
        _db.CalculationRuns.Add(run);
        await _db.SaveChangesAsync(ct);
        return run.Id;
    }

    public async Task<CalculationRun?> GetRunAsync(Guid runId, string userId, CancellationToken ct = default)
    {
        return await _db.CalculationRuns
            .Include(x => x.InputDataSet)
            .FirstOrDefaultAsync(x => x.Id == runId && x.InputDataSet.OwnerId == userId, ct);
    }

    public async Task<List<CalculationRun>> GetRunsForDatasetAsync(Guid datasetId, string userId, CancellationToken ct = default)
    {
        var ds = await _db.InputDataSets.FirstOrDefaultAsync(x => x.Id == datasetId && x.OwnerId == userId, ct);
        if (ds is null) return [];
        var items = await _db.CalculationRuns.Where(x => x.InputDataSetId == datasetId).ToListAsync(ct);
        return items.OrderByDescending(x => x.ExecutedAt).ToList();
    }

    public Task<byte[]?> GetRunPdfAsync(Guid runId, string userId, CancellationToken ct = default)
        => Task.FromResult<byte[]?>(null);

    public async Task<bool> CanAccessDataSetAsync(Guid id, string userId, bool isAdmin, CancellationToken ct = default)
    {
        if (isAdmin) return await _db.InputDataSets.AnyAsync(x => x.Id == id, ct);
        return await _db.InputDataSets.AnyAsync(x => x.Id == id && x.OwnerId == userId, ct);
    }
}
