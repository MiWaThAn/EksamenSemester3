using Application.DTO;
using Domain.Entity.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services.Sync
{
    public interface IEntitySyncHandler
    {
        IntegrationEntityType TargetType { get; }
        Task ProcessAndSaveAsync(List<BaseIntegrationDTO> dtos, Guid companyId);
    }
}
