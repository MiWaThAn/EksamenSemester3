using Application.Interfaces;
using MediatR;
using Shared.Item.Registrations.Commands.Time;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Item.Registrations.Time
{
    public class TakeBreakHandler : IRequestHandler<TakeBreakCommand, BaseRegistrationResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        public TakeBreakHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<BaseRegistrationResponse> Handle(TakeBreakCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, cancellationToken);
                var employee = await _unitOfWork.Employees.GetByIdAsync(request.EmployeeId, cancellationToken);
                if (employee == null)
                    return new BaseRegistrationResponse { Success = false, Message = "Medarbejder ikke fundet." };
                var worklog = await _unitOfWork.WorkLogs.GetActiveByEmployeeIdAsync(employee.Id, cancellationToken);
                if (worklog == null)
                    return new BaseRegistrationResponse { Success = false, Message = "Arbejdslog ikke fundet." };
                worklog.TakeBreak(employee);
                await _unitOfWork.CompleteAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return new BaseRegistrationResponse { Success = true, Message = "Pause..." };
            }
            catch (InvalidOperationException ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new BaseRegistrationResponse { Success = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                // TODO: _logger.LogError(ex, "Fejl ved tage pause for Employee {EmployeeId}", request.EmployeeId);
                return new BaseRegistrationResponse { Success = false, Message = "Der opstod en uventet systemfejl." };
            }
        }
    }
}
