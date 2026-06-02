using Application.Interfaces;
using MediatR;
using Shared.Item.Registrations.Commands;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Item.Registrations
{
    public class SwitchProjectActivityHandler : IRequestHandler<SwitchActivityCommand, BaseRegistrationResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        public SwitchProjectActivityHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<BaseRegistrationResponse> Handle(SwitchActivityCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, cancellationToken);
                var emp = await _unitOfWork.Employees.GetByIdAsync(request.EmployeeId, cancellationToken);
                if (emp == null)
                    return BaseRegistrationResponse.Fail("Employee not found.");
                var worklog = await _unitOfWork.WorkLogs.GetActiveByEmployeeIdAsync(emp.Id, cancellationToken);
                if (worklog == null)
                    return BaseRegistrationResponse.Fail("Work log not found.");
                var projectActivity = await _unitOfWork.ProjectActivities.GetByIdAsync(request.NewProjectActivityId, cancellationToken);
                if (projectActivity == null)
                    return BaseRegistrationResponse.Fail("Project activity not found.");
                var active = worklog.SwitchActivity(projectActivity, emp);
                await _unitOfWork.HourRegistrations.AddAsync(active);
                await _unitOfWork.CompleteAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return BaseRegistrationResponse.Ok(active.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return BaseRegistrationResponse.Fail(ex.Message);
            }
        }
    }
}
