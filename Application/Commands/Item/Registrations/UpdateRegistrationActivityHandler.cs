using Application.Interfaces;
using MediatR;
using Shared.Item.Registrations.Commands;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Item.Registrations
{
    public class UpdateRegistrationActivityHandler : IRequestHandler<UpdateRegistrationActivityCommand, BaseRegistrationResponse>
    {
        IUnitOfWork _unitOfWork;
        public UpdateRegistrationActivityHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<BaseRegistrationResponse> Handle(UpdateRegistrationActivityCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, cancellationToken);
                var account = await _unitOfWork.Accounts.GetByIdAsync(request.AccountId, cancellationToken);
                if (account == null)
                    return BaseRegistrationResponse.Fail("Account not found.");
                var workLog = await _unitOfWork.WorkLogs.GetByIdAsync(request.WorkLogId, cancellationToken);
                if (workLog == null)
                    return BaseRegistrationResponse.Fail("Work log not found.");
                if (!account.EmployeeId.HasValue)
                    return BaseRegistrationResponse.Fail("Account does not belong to an employee");
                if (account.EmployeeId.Value != workLog.EmployeeId)
                    return BaseRegistrationResponse.Fail("Account does not belong to the employee associated with the work log.");
                var emp = await _unitOfWork.Employees.GetByIdAsync(account.EmployeeId.Value, cancellationToken);
                if (emp == null)
                    return BaseRegistrationResponse.Fail("Employee not found.");
                var projectActivity = await _unitOfWork.ProjectActivities.GetByIdAsync(request.NewProjectActivityId, cancellationToken);
                if (projectActivity == null)
                    return BaseRegistrationResponse.Fail("Project activity not found.");
                workLog.UpdateActivity(projectActivity, emp, request.RegistrationId);
                await _unitOfWork.CompleteAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return BaseRegistrationResponse.Ok(request.RegistrationId);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return BaseRegistrationResponse.Fail(ex.Message);
            }
        }
    }
}
