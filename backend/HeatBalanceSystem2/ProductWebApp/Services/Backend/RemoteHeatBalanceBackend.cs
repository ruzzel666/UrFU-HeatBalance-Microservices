using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using HeatBalance.Contracts;
using Microsoft.AspNetCore.Authentication;
using ProductWebApp.Data;

namespace ProductWebApp.Services.Backend;

public sealed class RemoteHeatBalanceBackend : IHeatBalanceBackend
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public RemoteHeatBalanceBackend(
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    public async Task<List<InputDataSet>> GetDataSetsAsync(string userId, CancellationToken ct = default)
    {
        var dtos = await SendAsync<List<DatasetDto>>(HttpMethod.Get, "/api/datasets", ct);
        return dtos?.Select(MapDataset).OrderByDescending(x => x.UpdatedAt).ToList() ?? [];
    }

    public async Task<List<InputDataSet>> GetAllDataSetsAsync(CancellationToken ct = default)
        => await GetDataSetsAsync(string.Empty, ct);

    public async Task<InputDataSet?> GetDataSetAsync(Guid id, string userId, CancellationToken ct = default)
    {
        var dto = await SendAsync<DatasetDto>(HttpMethod.Get, $"/api/datasets/{id}", ct);
        if (dto is null) return null;
        if (dto.OwnerId != userId && !await IsAdminAsync()) return null;
        return MapDataset(dto);
    }

    public async Task<InputDataSet> CreateDataSetAsync(string userId, InputDataSet dataSet, CancellationToken ct = default)
    {
        var request = new CreateDatasetRequest
        {
            Name = dataSet.Name,
            CalculationType = (HeatBalance.Contracts.CalculationType)(int)dataSet.CalculationType,
            Comment = dataSet.Comment,
            InputJson = dataSet.InputJson
        };
        var dto = await SendAsync<DatasetDto>(HttpMethod.Post, "/api/datasets", ct, request)
                  ?? throw new InvalidOperationException("Failed to create dataset.");
        return MapDataset(dto);
    }

    public async Task UpdateDataSetAsync(string userId, InputDataSet dataSet, CancellationToken ct = default)
    {
        var request = new UpdateDatasetRequest
        {
            Name = dataSet.Name,
            Comment = dataSet.Comment,
            InputJson = dataSet.InputJson
        };
        await SendAsync<DatasetDto>(HttpMethod.Put, $"/api/datasets/{dataSet.Id}", ct, request);
    }

    public async Task DeleteDataSetAsync(Guid id, string userId, CancellationToken ct = default)
    {
        await SendAsync<object?>(HttpMethod.Delete, $"/api/datasets/{id}", ct);
    }

    public Task DeleteDataSetAsAdminAsync(Guid id, CancellationToken ct = default)
        => DeleteDataSetAsync(id, string.Empty, ct);

    public async Task<Guid> RunCalculationAsync(Guid datasetId, string userId, CancellationToken ct = default)
    {
        var response = await SendAsync<CreateRunResponse>(HttpMethod.Post, "/api/runs", ct, new CreateRunRequest
        {
            InputDataSetId = datasetId
        }) ?? throw new InvalidOperationException("Failed to run calculation.");
        return response.RunId;
    }

    public async Task<CalculationRun?> GetRunAsync(Guid runId, string userId, CancellationToken ct = default)
    {
        var dto = await SendAsync<CalculationRunDto>(HttpMethod.Get, $"/api/runs/{runId}", ct);
        if (dto is null) return null;
        var dataset = await GetDataSetAsync(dto.InputDataSetId, userId, ct);
        if (dataset is null && !await IsAdminAsync()) return null;
        return MapRun(dto, dataset);
    }

    public async Task<List<CalculationRun>> GetRunsForDatasetAsync(Guid datasetId, string userId, CancellationToken ct = default)
    {
        var dtos = await SendAsync<List<CalculationRunDto>>(HttpMethod.Get, $"/api/runs/by-dataset/{datasetId}", ct);
        return dtos?.Select(x => MapRun(x, null)).OrderByDescending(x => x.ExecutedAt).ToList() ?? [];
    }

    public async Task<byte[]?> GetRunPdfAsync(Guid runId, string userId, CancellationToken ct = default)
    {
        var client = await CreateClientAsync(ct);
        var gateway = GetGatewayBaseUrl();
        var response = await client.GetAsync($"{gateway}/api/reports/{runId}/pdf", ct);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadAsByteArrayAsync(ct);
    }

    public async Task<bool> CanAccessDataSetAsync(Guid id, string userId, bool isAdmin, CancellationToken ct = default)
    {
        if (isAdmin) return true;
        var ds = await GetDataSetAsync(id, userId, ct);
        return ds is not null;
    }

    private async Task<T?> SendAsync<T>(HttpMethod method, string path, CancellationToken ct, object? body = null)
    {
        var client = await CreateClientAsync(ct);
        var gateway = GetGatewayBaseUrl();
        using var request = new HttpRequestMessage(method, $"{gateway}{path}");
        if (body is not null)
            request.Content = JsonContent.Create(body);

        var response = await client.SendAsync(request, ct);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return default;
        response.EnsureSuccessStatusCode();

        if (method == HttpMethod.Delete)
            return default;

        return await response.Content.ReadFromJsonAsync<T>(cancellationToken: ct);
    }

    private async Task<HttpClient> CreateClientAsync(CancellationToken ct)
    {
        var token = await _httpContextAccessor.HttpContext!.GetTokenAsync("access_token");
        if (string.IsNullOrWhiteSpace(token))
            throw new InvalidOperationException("Access token is missing.");

        var client = _httpClientFactory.CreateClient("HeatBalanceGateway");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private string GetGatewayBaseUrl()
        => _configuration["Microservices:GatewayUrl"] ?? "http://localhost:5000";

    private Task<bool> IsAdminAsync()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        return Task.FromResult(user?.IsInRole(AuthConstants.AdminRole) == true);
    }

    private static InputDataSet MapDataset(DatasetDto dto) => new()
    {
        Id = dto.Id,
        OwnerId = dto.OwnerId,
        Name = dto.Name,
        CalculationType = (Data.CalculationType)(int)dto.CalculationType,
        Comment = dto.Comment,
        SchemaVersion = dto.SchemaVersion,
        InputJson = dto.InputJson,
        CreatedAt = dto.CreatedAt,
        UpdatedAt = dto.UpdatedAt
    };

    private static CalculationRun MapRun(CalculationRunDto dto, InputDataSet? dataset) => new()
    {
        Id = dto.Id,
        InputDataSetId = dto.InputDataSetId,
        ExecutedAt = dto.ExecutedAt,
        ResultJson = dto.ResultJson,
        Notes = dto.Notes,
        InputDataSet = dataset ?? new InputDataSet { Id = dto.InputDataSetId }
    };
}
