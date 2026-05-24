using Application.Interfaces;
using Domain.Entity.Item;
using Domain.Entity.Item.Activities;
using MediatR;
using Shared.Item.Registrations.Commands;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Item.Registrations
{
    internal class SwitchProjectHandler : IRequestHandler<SwitchProjectCommand, BaseRegistrationResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private Project? Project;
        private ProjectActivity? ProjectActivity;
        public SwitchProjectHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<BaseRegistrationResponse> Handle(SwitchProjectCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, cancellationToken);
                Project = await _unitOfWork.Projects.GetByIdAsync(request.NewProjectId, cancellationToken);
                if (Project == null)
                    return BaseRegistrationResponse.Fail("New project not found.");
                ProjectActivity = await _unitOfWork.ProjectActivities.GetByIdAsync(request.NewProjectActivityId, cancellationToken);
                if (ProjectActivity == null)
                    return BaseRegistrationResponse.Fail("New project activity not found.");
                var WorkLog = await _unitOfWork.WorkLogs.GetActiveByEmployeeIdAsync(request.EmployeeId, cancellationToken);
                if (WorkLog == null)
                    return BaseRegistrationResponse.Fail("No active work log found for the employee.");
                WorkLog.SwitchProjectAndActivity(Project, ProjectActivity, null);
                await _unitOfWork.CompleteAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return BaseRegistrationResponse.Ok(WorkLog.ActiveRegistrationId.Value);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return BaseRegistrationResponse.Fail(ex.Message);
            }
        }
    }
}
