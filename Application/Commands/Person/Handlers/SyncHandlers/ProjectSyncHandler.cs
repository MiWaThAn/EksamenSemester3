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


        private async Task<Project> CreateEntity(ISyncEntity syncEntity)
        {
            var company = await _unitOfWork.Companies.GetByIdAsync(syncEntity.CompanyId);

            var projectSync =
                (SyncEntity<ProjectDTO>)syncEntity;

            var dto = projectSync.Data;

           var projectBuilder =  new ProjectBuilder()
            .WithName(dto.Name)
            .WithIsStatus(dto.IsClosed ? Status.Lukket : Status.Åben)
            .WithDescription(string.Empty);
            
            return company.CreateProject(projectBuilder);
        }



        public async Task CreateAsync(
            ISyncEntity syncEntity,
            IntegrationSetting setting,
            IntegrationEntityType entityType)
        {

           
            var project = await CreateEntity(syncEntity);



                var mapping = setting.CreateMapping(
                    new IntegrationMappingBuilder()
                        .WithLocalId(project)
                        .WithEntityType(entityType)
                        .WithExternalId(syncEntity.ExternalId)
                        .WithObjectVersion(syncEntity.ObjectVersion));
                await _unitOfWork.Mappings.AddAsync(mapping);
                await _unitOfWork.Projects.AddAsync(project);
               
        }
           
        

        public async Task UpdateAsync(
            ISyncEntity syncEntity,
            IntegrationMapping mapping)
        {
            if (mapping.ObjectVersion == syncEntity.ObjectVersion)
                return;

            var dto =
                ((SyncEntity<ProjectDTO>)syncEntity).Data;

           

                var local = await _unitOfWork.Projects
                    .GetByIdAsync(mapping.LocalId);

                if (local == null)
                {
                   
                    return;
                }

            local.UpdateProjectName(dto.Name);

            
            if (dto.IsClosed && local.Status != Status.Lukket)
            {
                local.MarkAsClosed(); 
            }
            else if (!dto.IsClosed && local.Status == Status.Lukket)
            {
                local.MarkAsOpen();  
            }

            mapping.UpdateObjectVersion(
                    syncEntity.ObjectVersion);
              
        }


    }
}
