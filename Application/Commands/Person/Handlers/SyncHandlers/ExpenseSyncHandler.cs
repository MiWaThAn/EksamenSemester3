using Application.DTO;
using Application.DTO.External;
using Application.Interfaces;
using Application.Interfaces.Handlers;
using Application.Interfaces.Services.Sync;
using Domain.Builders.Item;
using Domain.Builders.Mapping;
using Domain.Entity.Item;
using Domain.Entity.Mapping;
using Domain.Entity.Mapping.ValueObjects;
using Domain.Entity.Person;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Person.Handlers.SyncHandlers
{
    public class ExpenseSyncHandler : IEntitySyncHandler
    {
        private readonly IUnitOfWork _unitOfWork;
        public ExpenseSyncHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public bool CanHandle(IntegrationEntityType entityType)
        {
            return entityType.Value == "expense";
        }
        private async Task<Expense> CreateEntity(ISyncEntity syncEntity)
        {
            var company = await _unitOfWork.Companies.GetByIdAsync(syncEntity.CompanyId);
            if (company == null)
            {
                throw new Exception($"Company with ID {syncEntity.CompanyId} not found.");
            }
            var expenseSync =
                (SyncEntity<ExpenseDTO>)syncEntity;
            var expenseBuilder = new ExpenseBuilder()
                .WithName(expenseSync.Data.Name);
            return company.CreateExpense(expenseBuilder);
        }

        public async Task CreateAsync(ISyncEntity syncEntity, IntegrationSetting setting, IntegrationEntityType entityType)
        {
            var expenseSync =
                (SyncEntity<ExpenseDTO>)syncEntity;
            if (expenseSync.Data.IsBarred == true)
            {
                return;
            }
            var mappings = await _unitOfWork.Mappings.GetByExternalId(syncEntity.ExternalId, entityType);
            if (mappings.Any())
            {
                return;
            }

            
                var expense = await CreateEntity(syncEntity);
                var mapping = setting.CreateMapping(
                    new IntegrationMappingBuilder()
                        .WithLocalId(expense)
                        .WithEntityType(entityType)
                        .WithExternalId(syncEntity.ExternalId)
                        .WithObjectVersion(syncEntity.ObjectVersion));
                await _unitOfWork.Mappings.AddAsync(mapping);
                await _unitOfWork.Expenses.AddAsync(expense);
               



        }
        public async Task UpdateAsync(ISyncEntity syncEntity, IntegrationMapping mapping)
        {
            var expenseSync =
                (SyncEntity<ExpenseDTO>)syncEntity;

            if (mapping.ObjectVersion == syncEntity.ObjectVersion)
            {
                return;
            }
           
                var local = await _unitOfWork.Expenses.GetByIdAsync(mapping.LocalId);
                if (local == null)
                {
                    throw new Exception($"Expense with ID {mapping.LocalId} not found.");
                }
                if (local.Name != expenseSync.Data.Name)
                {
                    local.UpdateExpenseName(expenseSync.Data.Name);
                }
                if (mapping.ExternalId != syncEntity.ExternalId)
                {
                    mapping.UpdateExternalId(syncEntity.ExternalId);
                }

                mapping.UpdateObjectVersion(syncEntity.ObjectVersion);
              
        }
    }
}