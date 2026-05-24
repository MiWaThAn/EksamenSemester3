using Domain.Entity.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.External
{
    public class ProviderDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public static ProviderDTO FromEntity(Provider provider) => new()
        {
            Id = provider.Id,
            Name = provider.Datasource.Value
        };
    }
}
