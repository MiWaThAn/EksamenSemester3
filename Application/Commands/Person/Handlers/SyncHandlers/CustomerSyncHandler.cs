using Application.DTO;
using Application.Interfaces;
using Application.Interfaces.Handlers;
using Application.Interfaces.Services.Sync;
using Domain.Builders.Mapping;
using Domain.Entity.Item;
using Domain.Entity.Mapping;
using Domain.Entity.Mapping.ValueObjects;
using Domain.Entity.Person;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Commands.Person.Handlers.SyncHandlers
{
    public class CustomerSyncHandler : IEntitySyncHandler
    {

        private readonly IUnitOfWork _unitOfWork;


        public CustomerSyncHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public bool CanHandle(IntegrationEntityType entityType)
        {
            return entityType.Value == "customer";
        }
        
        public async Task CreateAsync(SyncEntity syncEntity,IntegrationSetting setting,IntegrationEntityType entityType)
        {
                var customer = (Customer)syncEntity.Entity;
            try { 
            await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);
                await _unitOfWork.Customers.AddAsync(customer);
            setting.CreateMapping(new IntegrationMappingBuilder()
                    .WithLocalId(customer)
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
            var customer = (Customer)syncEntity.Entity;
            try
            {
              var local = await _unitOfWork.Customers.GetByIdAsync(mapping.LocalId);
              if (local == null) return;
            await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);
                
              local.UpdateName(customer.Name);
                

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