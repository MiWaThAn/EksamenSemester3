using Application.DTO;
using Application.Interfaces;
using Application.Interfaces.Handlers;
using Domain.Builders.Mapping;
using Domain.Entity.Mapping;
using Domain.Entity.Mapping.ValueObjects;
using Domain.Entity.Person;
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

        public async Task CreateAsync(SyncEntity syncEntity, IntegrationSetting setting, IntegrationEntityType entityType)
        {
            var employee = (Employee)syncEntity.Entity;
            try
            {
                await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);
                await _unitOfWork.Employees.AddAsync(employee);
                setting.CreateMapping(new IntegrationMappingBuilder()
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
        public async Task UpdateAsync(SyncEntity syncEntity, IntegrationMapping mapping)
        {
            if (mapping.ObjectVersion == syncEntity.ObjectVersion)
                return;
            var employee = (Employee)syncEntity.Entity;
            try
            {
                await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);
                var local = await _unitOfWork.Employees.GetByIdAsync(mapping.LocalId);
                if (local == null) return;
                local.UpdateName(employee.Name);
                if (employee.Email != null)
                    local.UpdateEmail(employee.Email);


                mapping.UpdateObjectVersion(syncEntity.ObjectVersion);
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
