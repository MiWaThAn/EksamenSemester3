using Application.DTO;
using Application.Interfaces;
using Application.Interfaces.Handlers;
using Domain.Builders.Mapping;
using Domain.Entity.Item;
using Domain.Entity.Mapping;
using Domain.Entity.Mapping.ValueObjects;
using Domain.Entity.Person;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Person.Handlers.SyncHandlers
{
    public class ProjectSyncHandler : IEntitySyncHandler
    {
        private readonly IUnitOfWork _unitOfWork;


        public ProjectSyncHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public bool CanHandle(IntegrationEntityType entityType)
        {
            return entityType.Value == "project";
        }

        public async Task CreateAsync(SyncEntity syncEntity, IntegrationSetting setting, IntegrationEntityType entityType)
        {
            var project = (Project)syncEntity.Entity;
            try
            {
                await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);
                await _unitOfWork.Projects.AddAsync(project);
                setting.CreateMapping(new IntegrationMappingBuilder()
                        .WithLocalId(project)
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
            var project = (Project)syncEntity.Entity;
            try
            {
                var local = await _unitOfWork.Projects.GetByIdAsync(mapping.LocalId);
                if (local == null) return;
                await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

                local.UpdateProjectName(project.Name);
                


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
