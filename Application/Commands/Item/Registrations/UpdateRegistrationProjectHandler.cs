using Application.Interfaces;
using MediatR;
using Shared.Item.Registrations.Commands;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Item.Registrations
{
    public class UpdateRegistrationProjectHandler : IRequestHandler<UpdateRegistrationProjectCommand, BaseRegistrationResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateRegistrationProjectHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<BaseRegistrationResponse> Handle(UpdateRegistrationProjectCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, cancellationToken);
                var account = await _unitOfWork.Accounts.GetByIdAsync(request.AccountId, cancellationToken);
                if (account == null)
                    return BaseRegistrationResponse.Fail("Account not found.");
                if (account.EmployeeId == null)
                    return BaseRegistrationResponse.Fail("Account is not linked to an employee.");
                var emp = await _unitOfWork.Employees.GetByIdAsync(account.EmployeeId.Value, cancellationToken);
                if (emp == null)
                    return BaseRegistrationResponse.Fail("Employee not found.");
                var project = await _unitOfWork.Projects.GetByIdAsync(request.NewProjectId, cancellationToken);
                if (project == null)
                    return BaseRegistrationResponse.Fail("New project not found.");
                var projectActivity = await _unitOfWork.ProjectActivities.GetByIdAsync(request.NewProjectActivityId, cancellationToken);
                if (projectActivity == null)
                    return BaseRegistrationResponse.Fail("New project activity not found.");
                var workLog = await _unitOfWork.WorkLogs.GetByIdAsync(request.WorkLogId, cancellationToken);
                if (workLog == null)
                    return BaseRegistrationResponse.Fail("Work log not found.");
                workLog.UpdateProjectAndActivity(project, projectActivity, emp, request.RegistrationId);
                await _unitOfWork.CompleteAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return BaseRegistrationResponse.Ok(workLog.ActiveRegistrationId.Value);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return BaseRegistrationResponse.Fail(ex.Message);
            }
        }
    }
}
