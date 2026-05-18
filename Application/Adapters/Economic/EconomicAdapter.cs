using Application.DTO.External.Economic;
using Application.Interfaces.Adapters;
using Domain.Builders.Item;
using Domain.Builders.Person;
using Domain.Entity.Item;
using Domain.Entity.Mapping.ValueObjects;
using Domain.Entity.Person;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Application.Adapters.Economic
{
    public class EconomicAdapter : IProviderAdapter
    {
        
        
            public bool Supports(DataSource datasource) => datasource.Value == "economic";

            public IEnumerable<SyncEntity> Map(
                string json,
                IntegrationEntityType entityType,
                Guid companyId)
            {
                return entityType.Value switch
                {
                    "employee" => MapEmployees(json, companyId),
                    "project" => MapProjects(json, companyId),
                    "customer" => MapCustomers(json, companyId),
                    _ => throw new Exception(
                        $"Economic adapter does not support '{entityType}'.")
                };
            }

            private IEnumerable<SyncEntity> MapEmployees(string json, Guid companyId)
            {
                var response = JsonSerializer.Deserialize<EconomicEmployeeResponse>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return response!.Items
                    .Where(dto => !dto.IsBarred)
                    .Select(dto => new SyncEntity
                    {
                        ExternalId = dto.Number.ToString(),
                        ObjectVersion = dto.ObjectVersion,
                        Entity = new EmployeeBuilder()
                        .WithName(dto.Name)
                        .WithCompanyId(companyId)
                        .WithEmployeeType(EmployeeType.None)  
                        .WithAutonomy(false)
                        .WithEmail(dto.Email != null
                         ? new EmailAddress(dto.Email)
                        : null).Build()
                    });
            }

            private IEnumerable<SyncEntity> MapProjects(string json, Guid companyId)
            {
                var response = JsonSerializer.Deserialize<EconomicProjectResponse>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return response!.Items.Select(dto => new SyncEntity
                {
                    ExternalId = dto.Number.ToString(),
                    ObjectVersion = dto.ObjectVersion,
                    Entity = new ProjectBuilder()
                        .WithName(dto.Name)
                        .WithCompanyId(companyId)
                        .WithIsStatus(dto.IsClosed ? Status.Lukket : Status.Åben)
                        .WithDescription(string.Empty)
                        .Build()
                });
            }

            private IEnumerable<SyncEntity> MapCustomers(string json, Guid companyId)
            {
                var response = JsonSerializer.Deserialize<EconomicCustomerResponse>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return response!.Items.Select(dto => new SyncEntity
                {
                    ExternalId = dto.CustomerNumber.ToString(),
                    ObjectVersion = dto.ObjectVersion,
                    Entity = new CustomerBuilder()
                    .WithName(dto.Name)
                    .WithEmail(dto.Email != null
                     ? new EmailAddress(dto.Email)
                     : null).Build()
                });
            }
        }






    }
