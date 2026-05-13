using Domain.Builders.Mapping;
using Domain.Entity.Mapping;
using Domain.Guards;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Interfaces.Mapping;
using Domain.Entity.Mapping.ValueObjects;
using Domain.Interfaces.Repos;
namespace Domain.Services.Mapping
{
    

    public class ProviderFactory : IProviderFactory
    {
        private readonly IProviderRepository _repository;

        public ProviderFactory(IProviderRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<Result<Provider>> CreateAsync(DataSource datasource, Dictionary<IntegrationEntityType, string> urls)
        {
           Guard.AgainstNull(datasource, nameof(datasource));
            Guard.AgainstNull(urls, nameof(urls));
            
           
            var existing = await _repository.FindByDatasourceAsync(datasource);
            if (existing != null)
            {
                return Result<Provider>.Failure($"Provider for {datasource} eksisterer allerede."); 
            }

            
            var provider = ProviderFactory.CreateFromConfig(datasource, urls); 

            return Result<Provider>.Success(provider);
        }
        internal static Provider CreateFromConfig(DataSource datasource, Dictionary<IntegrationEntityType, string> urls)
        {
            return new ProviderBuilder()
                .WithDataSource(datasource)
                .WithUrls(urls)  
                .Build();
        }

    }
}
