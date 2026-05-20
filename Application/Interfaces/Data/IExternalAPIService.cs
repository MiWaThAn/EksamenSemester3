using Domain.Entity.Mapping;
using Domain.Entity.Mapping.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Data
{
    public interface IExternalAPIService
    {
        
        public Task<string> FetchFromAPI(string url, IntegrationCredential credential);


    }
}
