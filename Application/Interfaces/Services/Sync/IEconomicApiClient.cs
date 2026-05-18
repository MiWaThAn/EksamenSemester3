using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services.Sync
{
    public interface IEconomicApiClient
    {
        Task<string> GetCustomerAsync(string customerId);
    }
}
