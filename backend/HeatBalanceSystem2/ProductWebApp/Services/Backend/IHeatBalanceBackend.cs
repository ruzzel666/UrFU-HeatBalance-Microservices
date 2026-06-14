using ProductWebApp.Data;

namespace ProductWebApp.Services.Backend;

public interface IHeatBalanceBackend
{
    Task<List<InputDataSet>> GetDataSetsAsync(string userId, CancellationToken ct = default);
    Task<List<InputDataSet>> GetAllDataSetsAsync(CancellationToken ct = default);
    Task<InputDataSet?> GetDataSetAsync(Guid id, string userId, CancellationToken ct = default);
    Task<InputDataSet> CreateDataSetAsync(string userId, InputDataSet dataSet, CancellationToken ct = default);
    Task UpdateDataSetAsync(string userId, InputDataSet dataSet, CancellationToken ct = default);
    Task DeleteDataSetAsync(Guid id, string userId, CancellationToken ct = default);
    Task<Guid> RunCalculationAsync(Guid datasetId, string userId, CancellationToken ct = default);
    Task<CalculationRun?> GetRunAsync(Guid runId, string userId, CancellationToken ct = default);
    Task<List<CalculationRun>> GetRunsForDatasetAsync(Guid datasetId, string userId, CancellationToken ct = default);
    Task<byte[]?> GetRunPdfAsync(Guid runId, string userId, CancellationToken ct = default);
    Task DeleteDataSetAsAdminAsync(Guid id, CancellationToken ct = default);
    Task<bool> CanAccessDataSetAsync(Guid id, string userId, bool isAdmin, CancellationToken ct = default);
}
