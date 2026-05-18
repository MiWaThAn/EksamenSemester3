using Domain.Entity.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Data
{
    public interface IExternalAPIService
    {
        
        public Task<string> FetchFromAPI(string url, string key, string encryptedValue);


    }
}
