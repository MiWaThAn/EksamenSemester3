using Domain.Entity.Item;
using Domain.Entity.Item.Activities;
using Domain.Entity.Item.Registrations;
using Domain.Entity.Person;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
    public interface IRegistrationHandlerService
    {
        Task<Project> ValidateAndGetProjectAsync(Guid projectId);
        Task<ProjectActivity> ValidateAndGetProjectActivityAsync(Guid projectActivityId);
        Task<Company> ValidateAndGetCompanyConnectionForApproval(Guid companyId, bool isTime);
        Task<WorkLog> ValidateAndGetWorkLogAsync(Guid workLogId);
    }
}
