using Application.DTO;
using Application.DTO.External;
using Application.Interfaces;
using Application.Interfaces.Handlers;
using Application.Interfaces.Services.Sync;
using Domain.Builders.Item;
using Domain.Builders.Mapping;
using Domain.Entity.Item.Activities;
using Domain.Entity.Mapping;
using Domain.Entity.Mapping.ValueObjects;


namespace Application.Commands.Person.Handlers.SyncHandlers
{
    public class ActivitySyncHandler : IEntitySyncHandler
    {

        private readonly IUnitOfWork _unitOfWork;
        public ActivitySyncHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public bool CanHandle(IntegrationEntityType entityType)
        {
            return entityType.Value == "activity";
        }

        private async Task<Activity> CreateEntity(ISyncEntity syncEntity)
        {
            var activitySync =
                (SyncEntity<ActivityDTO>)syncEntity;
            var company = await _unitOfWork.Companies.GetByIdAsync(activitySync.CompanyId);
            if (company is null)
            {
                throw new Exception($"Company with id {activitySync.CompanyId} not found.");
            }
            var dto = activitySync.Data;
            var activityBuilder = new ActivityBuilder()
                .WithName(dto.Name)
                .WithDescription(string.Empty);
            return company.CreateActivity(activityBuilder);


        }


        public async Task CreateAsync(ISyncEntity syncEntity, IntegrationSetting setting, IntegrationEntityType entityType)
        {

            var dto = ((SyncEntity<ActivityDTO>)syncEntity).Data;
            if (dto.IsBarred)
            {
                return;
            }
            var activityMappings = await _unitOfWork.Mappings.GetByExternalId(syncEntity.ExternalId, entityType);
             if (activityMappings.Any())
            {
            return;
            }
            
               
                var activity = await CreateEntity(syncEntity);

                var mapping = setting.CreateMapping(new IntegrationMappingBuilder()
                    .WithLocalId(activity)
                    .WithEntityType(entityType)
                    .WithExternalId(syncEntity.ExternalId)
                    .WithObjectVersion(syncEntity.ObjectVersion));
                await _unitOfWork.Mappings.AddAsync(mapping);
                await _unitOfWork.Activities.AddAsync(activity);
                

            
           
        }
        public async Task UpdateAsync(ISyncEntity syncEntity, IntegrationMapping mapping)
        {
            var activitySync =
                (SyncEntity<ActivityDTO>)syncEntity;
            var dto = activitySync.Data;
            if (activitySync.Data.IsBarred)
            {
                return;
            }
            if (mapping.ObjectVersion == syncEntity.ObjectVersion)
            {
                return;
            }
           
                var local = await _unitOfWork.Activities.GetByIdAsync(mapping.LocalId);
                if (local is null)
                {
                    throw new Exception($"Activity with id {mapping.LocalId} not found.");
                }
                if (dto.Name != local.Name)
                {
                    local.UpdateActivityName(dto.Name);
                }


                if (mapping.ExternalId != syncEntity.ExternalId)
                {
                    mapping.UpdateExternalId(syncEntity.ExternalId);
                }
                mapping.UpdateObjectVersion(syncEntity.ObjectVersion);
               
                
                

            






        }
    }
}
