using Shared.Model;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using UI.Services.Auth;
using UI.Services.Integration;

public class IntegrationService : IIntegrationService
{
    private readonly HttpClient _http;
    private readonly ISecureStorage _secureStorage;
    private readonly IAuthService _authService;

    public IntegrationService(HttpClient http, ISecureStorage secureStorage, IAuthService authService)
    {
        _http = http;
        _secureStorage = secureStorage;
        _authService = authService;
    }

    public async Task<List<ProviderModel>> GetProvidersAsync()
    {
        await EnsureAuthorizationHeaderAsync();
        return await _http.GetFromJsonAsync<List<ProviderModel>>("api/integrationsetting/providers") ?? new();
    }

    public async Task<bool> CreateIntegrationSettingAsync(string providerName, string keyName, string keyValue, List<string> entityTypes)
    {
        await EnsureAuthorizationHeaderAsync();

        var token = await _secureStorage.GetAsync("auth_token");
        var accountId = _authService.GetUserId(token);

        if (string.IsNullOrWhiteSpace(accountId)) return false;

        var response = await _http.PostAsJsonAsync("api/integrationsetting", new
        {
            accountId = Guid.Parse(accountId),
            providerName,
            keyName,
            keyValue,
            selectedEntityTypes = entityTypes
        });

        return response.IsSuccessStatusCode;
    }

    private async Task EnsureAuthorizationHeaderAsync()
    {
        if (_http.DefaultRequestHeaders.Authorization == null)
        {
            var token = await _secureStorage.GetAsync("auth_token");
            if (!string.IsNullOrWhiteSpace(token))
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
    public async Task<bool> DeleteIntegrationSettingAsync(Guid settingId)
    {
        await EnsureAuthorizationHeaderAsync();

        var token = await _secureStorage.GetAsync("auth_token");
        var accountId = _authService.GetUserId(token);

        if (string.IsNullOrWhiteSpace(accountId)) return false;

        var response = await _http.DeleteAsync($"api/integrationsetting/{settingId}?accountId={accountId}");
        return response.IsSuccessStatusCode;
    }
    public async Task<List<IntegrationSettingModel>> GetMyIntegrationSettingsAsync()
    {
        await EnsureAuthorizationHeaderAsync();

        var token = await _secureStorage.GetAsync("auth_token");
        var accountId = _authService.GetUserId(token);

        if (string.IsNullOrWhiteSpace(accountId)) return new();

        return await _http.GetFromJsonAsync<List<IntegrationSettingModel>>($"api/integrationsetting/company/{accountId}") ?? new();
    }

    public async Task<bool> TriggerSyncAsync(Guid settingId)
    {
        await EnsureAuthorizationHeaderAsync();

        var token = await _secureStorage.GetAsync("auth_token");
        var accountId = _authService.GetUserId(token);

        if (string.IsNullOrWhiteSpace(accountId)) return false;

        var response = await _http.PostAsync($"api/integrationsetting/{settingId}/sync?accountId={accountId}", null);
        return response.IsSuccessStatusCode;
    }

}