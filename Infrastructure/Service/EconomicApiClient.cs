using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using Application.Interfaces.Services.Sync;

namespace Infrastructure.Service;

public class EconomicApiClient : IEconomicApiClient
{
    private readonly HttpClient _httpClient;

    public EconomicApiClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;

        _httpClient.BaseAddress = new Uri("https://restapi.e-conomic.com/");

        var appSecret = configuration["ExternalProviders:Economic:X-AppSecretToken"];
        var grantToken = configuration["ExternalProviders:Economic:X-AgreementGrantToken"];

        _httpClient.DefaultRequestHeaders.Add("X-AppSecretToken", appSecret);
        _httpClient.DefaultRequestHeaders.Add("X-AgreementGrantToken", grantToken);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<string> GetCustomerAsync(string customerId)
    {
        var response = await _httpClient.GetAsync("customers");

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Kunne ikke hente kunder fra e-conomic: {response.StatusCode}");
        }

        return await response.Content.ReadAsStringAsync();
    }
}