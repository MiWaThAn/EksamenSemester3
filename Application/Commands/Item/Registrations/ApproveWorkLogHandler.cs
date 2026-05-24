using Application.Interfaces;
using MediatR;
using Shared.Item.Registrations.Commands;
using Shared.Item.Registrations.Events.Worklogs;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Item.Registrations
{
    public class ApproveWorkLogHandler : IRequestHandler<ApproveWorkLogCommand, BaseRegistrationResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        public ApproveWorkLogHandler(IUnitOfWork unitOfWork, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }
        public async Task<BaseRegistrationResponse> Handle(ApproveWorkLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, cancellationToken);
                var account = await _unitOfWork.Accounts.GetByIdAsync(request.AccountId, cancellationToken);
                if (account == null)
                    return BaseRegistrationResponse.Fail("Account not found.");
                if(!account.CompanyId.HasValue)
                    return BaseRegistrationResponse.Fail("Account is not associated with a company.");
                var workLog = await _unitOfWork.WorkLogs.GetByIdWithRegistrationsAsync(request.WorkLogId, cancellationToken);
                if (workLog == null)
                    return BaseRegistrationResponse.Fail("Work log not found.");
                var emp = await _unitOfWork.Employees.GetByIdAsync(workLog.EmployeeId, cancellationToken);
                if (emp == null)
                    return BaseRegistrationResponse.Fail("Employee not found.");
                if (emp.CompanyId != account.CompanyId.Value)
                    return BaseRegistrationResponse.Fail("Work log does not belong to the specified company.");
                var company = await _unitOfWork.Companies.GetByIdAsync(account.CompanyId.Value, cancellationToken);
                if (company == null)
                    return BaseRegistrationResponse.Fail("Company not found.");
                workLog.Approve(company);
                string dateOfWork = workLog.CreatedAt.ToString("dd/MM/yyyy");
                await _unitOfWork.CompleteAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                await _mediator.Publish(new WorkLogApprovedEvent(workLog.Id, emp.Id, dateOfWork), cancellationToken);
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
