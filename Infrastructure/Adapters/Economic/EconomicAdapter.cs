using Application.DTO;
using Application.DTO.External;
using Application.Interfaces.Adapters;
using Application.Interfaces.Services.Sync;

using Domain.Entity.Mapping.ValueObjects;

using System.Text.Json;

namespace Infrastructure.Adapters.Economic
{
    public class EconomicAdapter : IProviderAdapter
    {
        
        
            public bool Supports(DataSource datasource) => datasource.Value == "economic";


            public IEnumerable<ISyncEntity> Map(
                string json,
                IntegrationEntityType entityType,
                Guid companyId)
            {
                return entityType.Value switch
                {
                    "employee" => MapEmployees(json, entityType, companyId),
                    "project" => MapProjects(json, entityType, companyId),
                    "customer" => MapCustomers(json, entityType, companyId),
                    "projectActivity" => MapProjectActivities(json, entityType, companyId),
                    _ => throw new Exception(
                        $"Economic adapter does not support '{entityType}'.")
                };
            }
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };
        private IEnumerable<SyncEntity<EmployeeDTO>> MapEmployees(string json, IntegrationEntityType entityType, Guid companyId)
        {
            var response = JsonSerializer.Deserialize<EmployeeDTOResponse>(
                json, _jsonOptions);
            return response!.Items
                .Where(dto => !dto.IsBarred)
                .Select(dto => new SyncEntity<EmployeeDTO>
                {
                    ExternalId = dto.Number.ToString(),
                    ObjectVersion = dto.ObjectVersion,
                    CompanyId = companyId,
                    ObjectType = entityType,
                    Data = new EmployeeDTO
                    {
                        Name = dto.Name,
                        Email = dto.Email
                    }
                });
        }

        private IEnumerable<SyncEntity<ProjectDTO>> MapProjects(string json, IntegrationEntityType entityType, Guid companyId)
        {
            var response = JsonSerializer.Deserialize<ProjectDTOResponse>(
                json, _jsonOptions);

            return response!.Items.Select(dto => new SyncEntity<ProjectDTO>
            {
                ExternalId = dto.Number.ToString(),
                ObjectVersion = dto.ObjectVersion,
                CompanyId = companyId,
                ObjectType = entityType,
                Data = new ProjectDTO
                {
                    Name = dto.Name,
                    IsClosed = dto.IsClosed
                }
            });
        }
        private IEnumerable<SyncEntity<CustomerDTO>> MapCustomers(string json, IntegrationEntityType entityType, Guid companyId)
        {
            var response = JsonSerializer.Deserialize<CustomerDTOResponse>(
                json, _jsonOptions);

            return response!.Items.Select(dto => new SyncEntity<CustomerDTO>
            {
                ExternalId = dto.Number.ToString(),
                ObjectVersion = dto.ObjectVersion,
                CompanyId = companyId,
                ObjectType = entityType,
                Data = new CustomerDTO
                {
                    Name = dto.Name,
                    Email = dto.Email
                }
            });
        }
        private IEnumerable<SyncEntity<ProjectActivityDTO>> MapProjectActivities(string json, IntegrationEntityType entityType, Guid companyId)
        {             var response = JsonSerializer.Deserialize<ProjectActivityDTOResponse>(
                json, _jsonOptions);
            return response!.Items.Select(dto => new SyncEntity<ProjectActivityDTO>
            {
                ExternalId = dto.Number.ToString(),
                ObjectVersion = dto.ObjectVersion,
                CompanyId = companyId,
                ObjectType = entityType,
                Data = new ProjectActivityDTO
                {
                    
                    ProjectExternalId = dto.ProjectExternalId,
                    ActivityExternalId = dto.ActivityExternalId,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    ResponsibleEmployeeExternalId = dto.ResponsibleEmployeeExternalId,
                    Completed = dto.Completed
                }
            });
        }


    }
}






    
