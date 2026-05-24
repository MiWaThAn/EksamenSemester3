using Application.Interfaces;
using MediatR;
using Shared.Item.Registrations.Commands;
using Shared.Item.Registrations.Commands.Expenses;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Item.Registrations
{
    public class SubmitWorkLogHandler : IRequestHandler<SubmitWorkLogCommand, BaseRegistrationResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        public SubmitWorkLogHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<BaseRegistrationResponse> Handle(SubmitWorkLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, cancellationToken);
                var workLog = await _unitOfWork.WorkLogs.GetByIdWithRegistrationsAsync(request.WorkLogId, cancellationToken);
                if (workLog == null)
                    return BaseRegistrationResponse.Fail("Work log not found.");
                if (workLog.EmployeeId != request.EmployeeId)
                    return BaseRegistrationResponse.Fail("Unauthorized access to work log.");
                var emp = await _unitOfWork.Employees.GetByIdAsync(request.EmployeeId, cancellationToken);
                if (emp == null)
                    return BaseRegistrationResponse.Fail("Employee not found.");
                workLog.SubmitForApproval(emp);
                await _unitOfWork.CompleteAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return BaseRegistrationResponse.Ok(workLog.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return BaseRegistrationResponse.Fail(ex.Message);
            }
        }
    }
}
