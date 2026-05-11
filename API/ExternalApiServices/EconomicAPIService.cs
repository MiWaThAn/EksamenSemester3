
//using Domain.Interfaces;


//namespace API.ExternalApiServices
//{
//    public class EconomicAPIService : IExternalAPIService
//    {
//        private readonly IHttpClientFactory _httpClientFactory;
//        public EconomicAPIService(IHttpClientFactory httpClientFactory, )
//        {
//            _httpClientFactory = httpClientFactory;
//        }

//        public async Task<IEnumerable<CustomerDTO>> GetCustomersAsync(//modtag setting)
//        {
//            var client = _httpClientFactory.CreateClient();
//            var response = await client.GetAsync();




//        }

//        public Task<IEnumerable<ProjectDTO>> GetProjectsAsync(//modtag setting)
//        {
//            var client = _httpClientFactory.CreateClient();
//            var response = await client.GetAsync()
//        }
//        public Task<IEnumerable<EmployeeDTO>> GetEmployeesAsync(//modtag setting)
//        {
//            var client = _httpClientFactory.CreateClient();
//            var response = await client.GetAsync()
//        }
//    }
//}
