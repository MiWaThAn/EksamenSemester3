using Application.Interfaces.Data;
using Domain.Entity.Mapping;
using Domain.Entity.Mapping.ValueObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
namespace Infrastructure.Service
{
    public class ExternalAPIService : IExternalAPIService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        public ExternalAPIService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }



        public async Task<string> FetchFromAPI(string url, IntegrationCredential credential)
        {
            var client = _httpClientFactory.CreateClient();

            var appSecret = _configuration["ExternalProviders:Economic:X-AppSecretToken"];
            client.DefaultRequestHeaders.Add($"X-AppSecretToken", appSecret);
            client.DefaultRequestHeaders.Add($"{credential.Key}", credential.Value);


            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Kunne ikke hente kunder fra e-conomic: {response.StatusCode}");
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }


    }
}
