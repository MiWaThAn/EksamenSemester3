using Application.Interfaces;
using MediatR;
using Shared.Item.Registrations.Commands.Time;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Item.Registrations.Time
{
    public class EndWorkHandler : IRequestHandler<EndWorkCommand, BaseRegistrationResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        public EndWorkHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<BaseRegistrationResponse> Handle(EndWorkCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, cancellationToken);
                var Worklog = await _unitOfWork.WorkLogs.GetActiveByEmployeeIdAsync(request.EmployeeId, cancellationToken);
                if (Worklog == null)
                    return new BaseRegistrationResponse { Success = false, Message = "Aktiv arbejdslog ikke fundet for medarbejderen." };
                var employee = await _unitOfWork.Employees.GetByIdAsync(request.EmployeeId, cancellationToken);
                if (employee == null)
                    return new BaseRegistrationResponse { Success = false, Message = "Medarbejder ikke fundet." };
                Worklog.EndWork(employee);
                await _unitOfWork.CompleteAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return new BaseRegistrationResponse { Success = true, Message = "Arbejdet er afsluttet." };
            }
            catch (InvalidOperationException ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new BaseRegistrationResponse { Success = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new BaseRegistrationResponse { Success = false, Message = "En fejl opstod under afslutning af arbejdet." };
            }
        }
    }
}
