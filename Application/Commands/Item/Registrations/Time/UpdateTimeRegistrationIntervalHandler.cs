using Application.Interfaces;
using Domain.Entity.Item.Registrations;
using MediatR;
using Shared.Item.Registrations.Commands.Time;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Item.Registrations.Time
{
    public class UpdateTimeRegistrationIntervalHandler : IRequestHandler<UpdateTimeRegistrationIntervalCommand, BaseRegistrationResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateTimeRegistrationIntervalHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<BaseRegistrationResponse> Handle(UpdateTimeRegistrationIntervalCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, cancellationToken);
                var account = await _unitOfWork.Accounts.GetByIdAsync(request.AccountId, cancellationToken);
                if (account == null)
                    return BaseRegistrationResponse.Fail("Account not found.");
                if (!account.EmployeeId.HasValue)
                    return BaseRegistrationResponse.Fail("Account is not associated with an employee.");
                var emp = await _unitOfWork.Employees.GetByIdAsync(account.EmployeeId.Value, cancellationToken);
                if (emp == null)
                    return BaseRegistrationResponse.Fail("Employee not found.");
                var workLog = await _unitOfWork.WorkLogs.GetByIdAsync(request.WorkLogId, cancellationToken);
                if (workLog == null)
                    return BaseRegistrationResponse.Fail("Work log not found.");
                workLog.UpdateActiveRegistrationInterval(request.NewStartTime, request.NewEndTime, emp, request.RegistrationId, request.IsBreak ? TimeType.Break : TimeType.Work);
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
