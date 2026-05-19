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

            try
            {
                await _unitOfWork.BeginTransactionAsync(
                    System.Data.IsolationLevel.ReadCommitted);

                await _unitOfWork.Employees.AddAsync(employee);

                setting.CreateMapping(
                    new IntegrationMappingBuilder()
                        .WithLocalId(employee)
                        .WithEntityType(entityType)
                        .WithExternalId(syncEntity.ExternalId)
                        .WithObjectVersion(syncEntity.ObjectVersion));

                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }





        public async Task UpdateAsync(
            ISyncEntity syncEntity,
            IntegrationMapping mapping)
        {
            if (mapping.ObjectVersion == syncEntity.ObjectVersion)
                return;

            var dto =
                ((SyncEntity<EmployeeDTO>)syncEntity).Data;

            try
            {
                await _unitOfWork.BeginTransactionAsync(
                    System.Data.IsolationLevel.ReadCommitted);

                var local = await _unitOfWork.Employees
                    .GetByIdAsync(mapping.LocalId);

                if (local == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return;
                }

                local.UpdateName(dto.Name);

                if (dto.Email != null)
                {
                    local.UpdateEmail(
                        new EmailAddress(dto.Email));
                }

                mapping.UpdateObjectVersion(
                    syncEntity.ObjectVersion);

                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

    }
}
