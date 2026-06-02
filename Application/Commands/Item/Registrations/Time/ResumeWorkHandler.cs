using Application.Interfaces;
using MediatR;
using Shared.Item.Registrations.Commands.Time;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Item.Registrations.Time
{
    internal class ResumeWorkHandler : IRequestHandler<ResumeWorkCommand, BaseRegistrationResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ResumeWorkHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<BaseRegistrationResponse> Handle(ResumeWorkCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, cancellationToken);
                var Worklog = await _unitOfWork.WorkLogs.GetActiveByEmployeeIdAsync(request.EmployeeId, cancellationToken);
                if (Worklog == null)
                    return new BaseRegistrationResponse { Success = false, Message = "Aktiv worklog ikke fundet for medarbejder." };
                var employee = await _unitOfWork.Employees.GetByIdAsync(request.EmployeeId, cancellationToken);
                if (employee == null)
                    return new BaseRegistrationResponse { Success = false, Message = "Medarbejder ikke fundet." };
                Worklog.ResumeWork(employee);
                await _unitOfWork.CompleteAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return new BaseRegistrationResponse { Success = true, Message = "Arbejdet er genoptaget." };
            }
            catch (InvalidOperationException ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new BaseRegistrationResponse { Success = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new BaseRegistrationResponse() { Success = false, Message = "Der skete en fejl under genoptagelse af arbejdet.", Errors = new Dictionary<string, string[]> { { "Exception", new[] { ex.Message } } } };
            }
        }
    }
}
