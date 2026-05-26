using Application.DTO;
using Application.DTO.External;
using Application.Interfaces;
using Application.Interfaces.Handlers;
using Application.Interfaces.Services.Sync;
using Domain.Builders.Mapping;
using Domain.Builders.Person;
using Domain.Entity.Mapping;
using Domain.Entity.Mapping.ValueObjects;
using Domain.Entity.Person;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Person.Handlers.SyncHandlers
{
    public class EmployeeSyncHandler : IEntitySyncHandler
    {
        private readonly IUnitOfWork _unitOfWork;


        public EmployeeSyncHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public bool CanHandle(IntegrationEntityType entityType)
        {
            return entityType.Value == "employee";
        }

        private async Task<Employee> CreateEntity(ISyncEntity syncEntity)
        {
            var company = await _unitOfWork.Companies.GetByIdAsync(syncEntity.CompanyId);
            var employeeSync =
                (SyncEntity<EmployeeDTO>)syncEntity;

            var dto = employeeSync.Data;

            var employeeBuilder = new EmployeeBuilder()
                .WithName(dto.Name)
                .WithEmployeeType(EmployeeType.None)
                .WithAutonomy(false)
                .WithEmail(
                    dto.Email != null
                        ? new EmailAddress(dto.Email)
                        : null);
                return company.CreateEmployee(employeeBuilder);
        }



        public async Task CreateAsync(
            ISyncEntity syncEntity,
            IntegrationSetting setting,
            IntegrationEntityType entityType)
        {

           
            var employee = await CreateEntity(syncEntity);

                

                var mapping = setting.CreateMapping(
                    new IntegrationMappingBuilder()
                        .WithLocalId(employee)
                        .WithEntityType(entityType)
                        .WithExternalId(syncEntity.ExternalId)
                        .WithObjectVersion(syncEntity.ObjectVersion));
                await _unitOfWork.Mappings.AddAsync(mapping);
                await _unitOfWork.Employees.AddAsync(employee);
               
        }





        public async Task UpdateAsync(
            ISyncEntity syncEntity,
            IntegrationMapping mapping)
        {
            if (mapping.ObjectVersion == syncEntity.ObjectVersion)
                return;

            var dto =
                ((SyncEntity<EmployeeDTO>)syncEntity).Data;

            

                var local = await _unitOfWork.Employees
                    .GetByIdAsync(mapping.LocalId);

                if (local == null)
                {
                   
                    return;
                }

                local.UpdateName(dto.Name);

            var incomingEmail = dto.Email != null ? new EmailAddress(dto.Email) : null;
            if (local.Email != incomingEmail)
            {
                local.UpdateEmail(incomingEmail); 
            }
            if (mapping.ExternalId != syncEntity.ExternalId)
                {
                    mapping.UpdateExternalId(syncEntity.ExternalId);
                }
                
                mapping.UpdateObjectVersion(syncEntity.ObjectVersion);
               
               
        }

    }
}
