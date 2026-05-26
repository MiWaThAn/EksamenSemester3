
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
            return entityType.Value == "projectactivity";
        }
        private async Task<ProjectActivity> CreateEntity(ISyncEntity syncEntity)
        {

            var projectActivitySync =
                (SyncEntity<ProjectActivityDTO>)syncEntity;
          
                var dto = projectActivitySync.Data;
                if (dto.ProjectExternalId == null)
                {
                    throw new Exception("Missing required external ID for project in projectactivity sync.");

                }
            var projectMappings = await _unitOfWork.Mappings.GetByExternalId(dto.ProjectExternalId, IntegrationEntityType.From("project"));
            var projectMapping = projectMappings?.FirstOrDefault();
            if (projectMapping == null)
            {
                throw new Exception($"Could not find mapping for project with external ID {dto.ProjectExternalId} in projectactivity sync.");
            }
            var project = await _unitOfWork.Projects.GetByIdAsync(projectMapping.LocalId);
            if (project == null)
            {
                throw new Exception($"Could not find local project with ID {projectMapping.LocalId} for projectactivity sync.");
            }


            var activityMappings = await _unitOfWork.Mappings.GetByExternalId(dto.ActivityExternalId, IntegrationEntityType.From("activity"));
            var activityMapping = activityMappings?.FirstOrDefault();
            if (activityMapping == null)
            {
                throw new Exception($"Could not find mapping for activity with external ID {dto.ActivityExternalId} in projectactivity sync.");
            }

            var activity = await _unitOfWork.Activities.GetByIdAsync(activityMapping.LocalId);
            if (activity == null)
            {
                throw new Exception($"Could not find local activity with ID {activityMapping.LocalId} for projectactivity sync.");
            }

            var projectActivityBuilder = new ProjectActivityBuilder()
                    .WithStatus(dto.Completed ? Status.Lukket : Status.Åben)
                    .WithStartAndEndDates(dto.StartDate, dto.EndDate)
                    .WithActivity(activity);
                return project.CreateProjectActivity(projectActivityBuilder);
            


        }
        public async Task CreateAsync(ISyncEntity syncEntity, IntegrationSetting setting, IntegrationEntityType entityType)
        {
           

                var projectActivity = await CreateEntity(syncEntity);

                var dto = ((SyncEntity<ProjectActivityDTO>)syncEntity).Data;

                if (dto.ResponsibleEmployeeExternalId != null)
                {
                    var employeeMappings =
                        await _unitOfWork.Mappings.GetByExternalId(
                            dto.ResponsibleEmployeeExternalId,
                            IntegrationEntityType.From("employee"));
                    if (employeeMappings == null)
                    {
                        throw new Exception("Could not find mapping for employee in projectactivity sync.");
                    }
                    var employeeMapping = employeeMappings.FirstOrDefault();

                    if (employeeMapping != null)
                    {
                        projectActivity.AssignResponsibleEmployee(
                            employeeMapping.LocalId);
                    }
                }


                var mapping = setting.CreateMapping(
                    new IntegrationMappingBuilder()
                    .WithLocalId(projectActivity)
                    .WithEntityType(entityType)
                    .WithExternalId(syncEntity.ExternalId)
                    .WithObjectVersion(syncEntity.ObjectVersion));


                await _unitOfWork.Mappings.AddAsync(mapping);
                await _unitOfWork.ProjectActivities.AddAsync(projectActivity);
                


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
               
        }





    }
}
