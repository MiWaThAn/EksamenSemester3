using Domain.Entity.Mapping.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity.Mapping;
namespace Domain.Interfaces.Repos
{
    public interface IProviderRepository : IGenericRepository<Provider>
    {

        Task<Provider?> FindByDatasourceAsync(DataSource datasource);

    }
}
