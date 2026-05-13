using Application.Commands.Person.Responses;
using Domain.Entity.Mapping;
using Domain.Entity.Mapping.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Person
{
    public record AddIntegrationSettingCommand : IRequest<AddIntagrationSettingReponse>
    {
        public Guid CompanyId { get; init; }
        public string Datasource { get; init; } = string.Empty;  
        public string Key { get; init; } = string.Empty;
        public string Value { get; init; } = string.Empty;
        public List<string> SelectedEntityTypes { get; init; } = new(); 
    }
}
