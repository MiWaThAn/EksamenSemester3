
using Application.DTO;
using Application.DTO.External;
using Application.Interfaces;
using Application.Interfaces.Handlers;
using Application.Interfaces.Services.Sync;
using Domain.Builders.Item;
using Domain.Builders.Mapping;
using Domain.Entity.Item;
using Domain.Entity.Item.Activities;
using Domain.Entity.Mapping;
using Domain.Entity.Mapping.ValueObjects;
using Domain.Entity.Person;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Person.Handlers.SyncHandlers
{
    public class ProjectActivitySyncHandler : IEntitySyncHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProjectActivitySyncHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool CanHandle(IntegrationEntityType entityType)
        {
            return entityType.Value == "projectActivity";
        }
        private async Task<ProjectActivity> CreateEntity(ISyncEntity syncEntity)
        {
            
            var projectActivitySync =
                (SyncEntity<ProjectActivityDTO>)syncEntity;
            try
            {

            var dto = projectActivitySync.Data;
            if (dto.ProjectExternalId == null)
            {
                throw new Exception("Missing required external ID for project in projectactivity sync.");
                
            }
            var projectMapping = await _unitOfWork.Mappings.GetByExternalId(dto.ProjectExternalId, IntegrationEntityType.From("project"));
            if (projectMapping == null)
            {
                throw new Exception("Could not find mapping for project in projectactivity sync.");
            }
            var project = await _unitOfWork.Projects.GetByIdAsync(projectMapping.FirstOrDefault().LocalId);
            if (project == null)
            {
                throw new Exception("Could not find project for projectactivity sync.");
            }
            //Loading activity
            
                var activityMapping = await _unitOfWork.Mappings.GetByExternalId(dto.ActivityExternalId, IntegrationEntityType.From("activity"));
            if (activityMapping == null)
            {
                throw new Exception("Could not find mapping for activity in projectactivity sync.");
            }
                var activity = await _unitOfWork.Activities.GetByIdAsync(activityMapping.FirstOrDefault().LocalId);
            if (activity == null)
            {
                throw new Exception("Could not find activity for projectactivity sync.");
            }
            
                var projectActivityBuilder = new ProjectActivityBuilder()
                    .WithStatus(dto.Completed ? Status.Lukket : Status.Åben)
                    .WithStartAndEndDates(dto.StartDate, dto.EndDate)
                    .WithActivity(activity);
            return project.CreateProjectActivity(projectActivityBuilder);
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating project activity builder in projectactivity sync.", ex);
            }


        }
        public async Task CreateAsync(ISyncEntity syncEntity, IntegrationSetting setting, IntegrationEntityType entityType)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(
                    System.Data.IsolationLevel.ReadCommitted);

                var projectActivity = await CreateEntity(syncEntity);

                var dto = ((SyncEntity<ProjectActivityDTO>)syncEntity).Data;

                if (dto.ResponsibleEmployeeExternalId != null)
                {
                    var employeeMappings =
                        await _unitOfWork.Mappings.GetByExternalId(
                            dto.ResponsibleEmployeeExternalId,
                            IntegrationEntityType.From("employee"));
                    if (employeeMappings == null) {
                        throw new Exception("Could not find mapping for employee in projectactivity sync.");
                    }
                    var employeeMapping = employeeMappings.FirstOrDefault();

                    if (employeeMapping != null)
                    {
                        projectActivity.AssignResponsibleEmployee(
                            employeeMapping.LocalId);
                    }
                }


                var mapping =setting.CreateMapping( 
                    new IntegrationMappingBuilder()
                    .WithLocalId(projectActivity)
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

            var dto = ((SyncEntity<ProjectActivityDTO>)syncEntity).Data;

                var projectActivity = await _unitOfWork.ProjectActivities
                    .GetByIdAsync(mapping.LocalId);

                if (projectActivity == null)
                {
                    return;
                }
            try
            {
                await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);


                if (projectActivity.StartDate != dto.StartDate || projectActivity.EndDate != dto.EndDate)
                {
                    projectActivity.UpdateStartAndEndDates(dto.StartDate, dto.EndDate);
                }
               
                if (dto.Completed == true && projectActivity.Status != Status.Lukket)
                {
                    projectActivity.MarkAsClosed();
                }
                if (dto.Completed == false && projectActivity.Status == Status.Lukket)
                {
                    projectActivity.MarkAsOpen();
                }

                
                if (dto.ResponsibleEmployeeExternalId != null)
                {
                    var employeeMapping = await _unitOfWork.Mappings.GetByExternalId(dto.ResponsibleEmployeeExternalId, IntegrationEntityType.From("employee"));

                    var emp = employeeMapping.FirstOrDefault();

                    if (emp != null)
                    {
                        projectActivity.AssignResponsibleEmployee(emp.LocalId);
                    }
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
