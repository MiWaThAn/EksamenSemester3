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
    public class RejectWorkLogHandler : IRequestHandler<RejectWorkLogCommand, BaseRegistrationResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        public RejectWorkLogHandler(IUnitOfWork unitOfWork, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }
        public async Task<BaseRegistrationResponse> Handle(RejectWorkLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, cancellationToken);
                var workLog = await _unitOfWork.WorkLogs.GetByIdWithRegistrationsAsync(request.WorkLogId, cancellationToken);
                if (workLog == null)
                    return BaseRegistrationResponse.Fail("Work log not found.");
                var emp = await _unitOfWork.Employees.GetByIdAsync(workLog.EmployeeId, cancellationToken);
                if (emp == null)
                    return BaseRegistrationResponse.Fail("Employee not found.");
                if (emp.CompanyId != request.CompanyId)
                    return BaseRegistrationResponse.Fail("Work log does not belong to the specified company.");
                var company = await _unitOfWork.Companies.GetByIdAsync(request.CompanyId, cancellationToken);
                if (company == null)
                    return BaseRegistrationResponse.Fail("Company not found.");
                workLog.Reject(company, request.reason);
                await _unitOfWork.CompleteAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                await _mediator.Publish(new WorkLogRejectedEvent(workLog.Id, emp.Id, request.reason), cancellationToken);
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
