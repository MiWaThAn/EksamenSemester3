using Application.Interfaces.Data;
using Application.Interfaces.Services;
using Domain.Entity.Mapping;
using Domain.Entity.Mapping.ValueObjects;
using Infrastructure.Service.Security;
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
        private readonly IEncryptionService _encryptionService;
        public ExternalAPIService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IEncryptionService encryptionService)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _encryptionService = encryptionService;
        }


        public async Task<string> FetchFromAPI(string url, IntegrationCredential credential)
        {
            var client = _httpClientFactory.CreateClient();

            var decryptedValue = _encryptionService.Decrypt(credential.EncryptedValue);

            
            client.DefaultRequestHeaders.Add("X-AgreementGrantToken", decryptedValue);

            //TODO gør disse to generisk evt
            var appSecret = _configuration["ExternalProviders:Economic:X-AppSecretToken"];
            client.DefaultRequestHeaders.Add("X-AppSecretToken", appSecret);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Kunne ikke hente kunder fra e-conomic: {response.StatusCode}");

            return await response.Content.ReadAsStringAsync();
        }


    }
}
