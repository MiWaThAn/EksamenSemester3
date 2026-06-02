using Application.Interfaces;
using MediatR;
using Shared.Item.Registrations.Commands;
using Shared.Item.Registrations.Commands.Expenses;
using Shared.Item.Registrations.Events.Worklogs;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Item.Registrations
{
    public class SubmitWorkLogHandler : IRequestHandler<SubmitWorkLogCommand, BaseRegistrationResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        public SubmitWorkLogHandler(IUnitOfWork unitOfWork, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }
        public async Task<BaseRegistrationResponse> Handle(SubmitWorkLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, cancellationToken);
                var emp = await _unitOfWork.Employees.GetByIdAsync(request.EmployeeId, cancellationToken);
                if (emp == null)
                    return BaseRegistrationResponse.Fail("Employee not found.");
                var workLog = await _unitOfWork.WorkLogs.GetByIdWithRegistrationsAsync(request.WorkLogId, cancellationToken);
                if (workLog == null)
                    return BaseRegistrationResponse.Fail("Work log not found.");
                if (workLog.EmployeeId != emp.Id)
                    return BaseRegistrationResponse.Fail("Unauthorized access to work log.");
                workLog.SubmitForApproval(emp);
                var company = await _unitOfWork.Companies.GetByIdAsync(emp.CompanyId, cancellationToken);
                await _unitOfWork.CompleteAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                await _mediator.Publish(new WorkLogSubmittedEvent(workLog.Id, company.AccountId), cancellationToken);
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
