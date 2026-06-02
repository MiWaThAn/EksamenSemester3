using Application.Interfaces;
using MediatR;
using Shared.Item.Registrations.Commands.Time;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Item.Registrations.Time
{
    public class ClockOutHandler : IRequestHandler<ClockOutCommand, BaseRegistrationResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ClockOutHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<BaseRegistrationResponse> Handle(ClockOutCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, cancellationToken);
                var worklog = await _unitOfWork.WorkLogs.GetActiveByEmployeeIdAsync(request.EmployeeId, cancellationToken);
                if (worklog == null)
                    return new BaseRegistrationResponse { Success = false, Message = "Ingen aktiv arbejdslog fundet for medarbejderen." };
                var employee = await _unitOfWork.Employees.GetByIdAsync(request.EmployeeId, cancellationToken);
                if (employee == null)
                    return new BaseRegistrationResponse { Success = false, Message = "Medarbejder ikke fundet." };
                worklog.ClockOut(employee);
                await _unitOfWork.CompleteAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return new BaseRegistrationResponse { Success = true, Message = "Regisrering lukket ud." };
            }
            catch (InvalidOperationException ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new BaseRegistrationResponse { Success = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new BaseRegistrationResponse { Success = false, Message = "Der skete en fejl under clock out processen: " + ex.Message };
            }
        }
    }
}
