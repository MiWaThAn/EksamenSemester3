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
                var account = await _unitOfWork.Accounts.GetByIdAsync(request.AccountId, cancellationToken);
                if (account == null)
                    return BaseRegistrationResponse.Fail("Account not found.");
                if(account.EmployeeId == null)
                    return BaseRegistrationResponse.Fail("Account is not linked to an employee.");
                var emp = await _unitOfWork.Employees.GetByIdAsync(account.EmployeeId.Value, cancellationToken);
                if (emp == null)
                    return BaseRegistrationResponse.Fail("Employee not found.");
                Project = await _unitOfWork.Projects.GetByIdAsync(request.NewProjectId, cancellationToken);
                if (Project == null)
                    return BaseRegistrationResponse.Fail("New project not found.");
                ProjectActivity = await _unitOfWork.ProjectActivities.GetByIdAsync(request.NewProjectActivityId, cancellationToken);
                if (ProjectActivity == null)
                    return BaseRegistrationResponse.Fail("New project activity not found.");
                var WorkLog = await _unitOfWork.WorkLogs.GetActiveByEmployeeIdAsync(emp.Id, cancellationToken);
                if (WorkLog == null)
                    return BaseRegistrationResponse.Fail("No active work log found for the employee.");
                var active = WorkLog.SwitchProjectAndActivity(Project, ProjectActivity, emp);
                await _unitOfWork.HourRegistrations.AddAsync(active);
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
