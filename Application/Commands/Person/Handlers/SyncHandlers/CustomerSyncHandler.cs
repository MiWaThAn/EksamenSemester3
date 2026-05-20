using Application.DTO;
using Application.DTO.External;
using Application.Interfaces;
using Application.Interfaces.Handlers;
using Application.Interfaces.Services.Sync;
using Domain.Builders.Mapping;
using Domain.Builders.Person;
using Domain.Entity.Item;
using Domain.Entity.Mapping;
using Domain.Entity.Mapping.ValueObjects;
using Domain.Entity.Person;
using Domain.ValueObjects;
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

        
        
        private async Task<Customer> CreateEntity(ISyncEntity syncEntity)
        {
            var customerSync =
                (SyncEntity<CustomerDTO>)syncEntity;

            var dto = customerSync.Data;
            
    
            return new CustomerBuilder()
                .WithName(dto.Name)
                .WithEmail(
                    dto.Email != null
                        ? new EmailAddress(dto.Email)
                        : null)
                .Build();
        }




        public async Task CreateAsync(ISyncEntity syncEntity,IntegrationSetting setting,IntegrationEntityType entityType)
        {
            var customer = await CreateEntity(syncEntity);
            try {
                var dto = ((SyncEntity<CustomerDTO>)syncEntity).Data;

                var projectMapping = await _unitOfWork.Mappings
                    .GetByExternalId(syncEntity.ExternalId, syncEntity.ObjectType);
                var local = projectMapping.FirstOrDefault(m => m.EntityType.Value == "project" && syncEntity.ExternalId == m.ExternalId);
                await _unitOfWork.Projects.GetByIdAsync(local.LocalId);
                if (projectMapping != null)
                {
                    var project =  await _unitOfWork.Projects
                        .GetByIdAsync(local.LocalId);

                     project.LinkToCustomer(customer);
                }



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
        public async Task UpdateAsync(ISyncEntity syncEntity, IntegrationMapping mapping)
        {
            if (mapping.ObjectVersion == syncEntity.ObjectVersion)
                return;
            var dto = ((SyncEntity<CustomerDTO>)syncEntity).Data;
            try
            {
              var local = await _unitOfWork.Customers.GetByIdAsync(mapping.LocalId);
              if (local == null) return;
            await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);
                
              local.UpdateName(dto.Name);
                if (dto.Email != null && dto.Email != local.Email.Value)
                {
                    local.UpdateContactInfo(new EmailAddress(dto.Email), local.PhoneNumber);
                }

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