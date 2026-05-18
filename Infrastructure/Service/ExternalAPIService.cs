using System;
using System.Collections.Generic;
using System.Text;
using Application.Interfaces.Data;
using Domain.Entity.Mapping;
using System.Net.Http;
namespace Infrastructure.Service
{
    public class ExternalAPIService : IExternalAPIService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ExternalAPIService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }



        public async Task<string> FetchFromAPI(string url, string key, string encryptedValue)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add($"{key}", encryptedValue);

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }


    }
}
