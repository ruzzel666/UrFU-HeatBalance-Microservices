using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using HeatBalance.Contracts;
using HeatBalance.Math;

namespace HeatBalance.RunService.Services;

public sealed class ServiceTokenProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private string? _cachedToken;
    private DateTimeOffset _expiresAt = DateTimeOffset.MinValue;

    public ServiceTokenProvider(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<string> GetTokenAsync(CancellationToken ct = default)
    {
        if (_cachedToken is not null && DateTimeOffset.UtcNow < _expiresAt.AddMinutes(-1))
            return _cachedToken;

        var authority = _configuration["Auth:Authority"] ?? "http://localhost:5001";
        var client = _httpClientFactory.CreateClient();
        var response = await client.PostAsync($"{authority.TrimEnd('/')}/connect/token",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = AuthConstants.RunServiceClientId,
                ["client_secret"] = _configuration["Auth:RunServiceSecret"] ?? AuthConstants.RunServiceClientSecret,
                ["scope"] = string.Join(' ', AuthConstants.RunServiceScopes)
            }), ct);

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: ct)
                      ?? throw new InvalidOperationException("Token response is empty.");

        _cachedToken = payload.AccessToken;
        _expiresAt = DateTimeOffset.UtcNow.AddSeconds(payload.ExpiresIn > 0 ? payload.ExpiresIn : 3600);
        return _cachedToken;
    }

    private sealed class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }
}

public sealed class FurnaceCalculationClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ServiceTokenProvider _tokenProvider;

    public FurnaceCalculationClient(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ServiceTokenProvider tokenProvider)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _tokenProvider = tokenProvider;
    }

    public async Task<HeatBalanceResult> CalculateAsync(DatasetDto dataset, CancellationToken ct = default)
    {
        var routePrefix = dataset.CalculationType.ToRoutePrefix();
        var baseUrl = _configuration[$"Services:Furnaces:{routePrefix}"]
                      ?? throw new InvalidOperationException($"Furnace service URL is not configured for '{routePrefix}'.");

        var token = await _tokenProvider.GetTokenAsync(ct);
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var content = new StringContent(dataset.InputJson, System.Text.Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"{baseUrl.TrimEnd('/')}/api/v1/calculate", content, ct);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(ct);
            throw new InvalidOperationException($"Furnace service error ({response.StatusCode}): {error}");
        }

        return await response.Content.ReadFromJsonAsync<HeatBalanceResult>(JsonOptions, ct)
               ?? throw new InvalidOperationException("Empty calculation result.");
    }
}

public sealed class DatasetApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ServiceTokenProvider _tokenProvider;

    public DatasetApiClient(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ServiceTokenProvider tokenProvider)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _tokenProvider = tokenProvider;
    }

    public async Task<DatasetDto> GetDatasetAsync(Guid id, string userAccessToken, CancellationToken ct = default)
    {
        var baseUrl = _configuration["Services:Dataset"] ?? "http://localhost:5100";
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userAccessToken);

        return await client.GetFromJsonAsync<DatasetDto>($"{baseUrl.TrimEnd('/')}/api/v1/datasets/{id}", ct)
               ?? throw new InvalidOperationException("Dataset not found.");
    }
}
