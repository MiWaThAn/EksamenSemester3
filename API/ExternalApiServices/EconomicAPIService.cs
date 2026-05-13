
//using Domain.Interfaces;

using Application.DTOs;
using Domain.Entity.Person;
using Application.Interfaces.Services;
namespace API.ExternalApiServices
{
    public class EconomicAPIService : IExternalAPIService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public EconomicAPIService(IHttpClientFactory httpClientFactory) 
        { 
            _httpClientFactory = httpClientFactory;
        }

        //public async Task<IEnumerable<CustomerDTO>> GetCustomersAsync(Account.Settings.Provider provider)
        //{
        //    var client = _httpClientFactory.CreateClient();
        //    var response = await client.GetAsync();

            public void DoNothing()
        {

        }


        //}

        //public Task<IEnumerable<ProjectDTO>> GetProjectsAsync(//modtag setting)
        //{
        //    var client = _httpClientFactory.CreateClient();
        //    var response = await client.GetAsync()
        //}
        //public Task<IEnumerable<EmployeeDTO>> GetEmployeesAsync(//modtag setting)
        //{
        //    var client = _httpClientFactory.CreateClient();
        //    var response = await client.GetAsync()
        //}
    }
}
