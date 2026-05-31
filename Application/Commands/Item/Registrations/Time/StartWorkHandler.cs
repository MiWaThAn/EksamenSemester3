using Application.Interfaces;
using Domain.Entity.Item.Registrations;
using Domain.Services;
using MediatR;
using Shared.Item.Registrations.Commands.Time;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Item.Registrations.Time
{
    public class StartWorkHandler : IRequestHandler<StartWorkCommand, BaseRegistrationResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        public StartWorkHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<BaseRegistrationResponse> Handle(StartWorkCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, cancellationToken);
                var account = await _unitOfWork.Accounts.GetByIdAsync(request.AccountId, cancellationToken);
                if(account == null)
                    return new BaseRegistrationResponse { Success = false, Message = "Konto ikke fundet." };
                if(!account.EmployeeId.HasValue)
                    return new BaseRegistrationResponse { Success = false, Message = "Konto er allerede tilknyttet en medarbejder." };
                var employee = await _unitOfWork.Employees.GetByIdAsync(account.EmployeeId.Value, cancellationToken);
                if (employee == null)
                    return new BaseRegistrationResponse { Success = false, Message = "Medarbejder ikke fundet." };
                var Worklog = await _unitOfWork.WorkLogs.GetActiveByEmployeeIdAsync(employee.Id, cancellationToken);
                if (Worklog == null)
                {
                    var emp = await _unitOfWork.Employees.GetByIdAsync(employee.Id, cancellationToken);
                    if (emp == null)
                        return new BaseRegistrationResponse { Success = false, Message = "Medarbejder ikke fundet." };
                    Worklog = emp.CreateWorkLog(new WorkLogBuilder());
                    await _unitOfWork.WorkLogs.AddAsync(Worklog, cancellationToken);
                }
                var project = await _unitOfWork.Projects.GetByIdAsync(request.projectId, cancellationToken);
                if (project == null)
                    return new BaseRegistrationResponse { Success = false, Message = "Projekt ikke fundet." };
                var activity = await _unitOfWork.ProjectActivities.GetByIdAsync(request.projectActivityId, cancellationToken);
                if (activity == null)
                    return new BaseRegistrationResponse { Success = false, Message = "Projektaktivitet ikke fundet." };
                var reg = Worklog.StartWork(project, activity,employee);
                await _unitOfWork.HourRegistrations.AddAsync(reg);
                await _unitOfWork.CompleteAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return new BaseRegistrationResponse { Success = true, Message = "Arbejdet er startet." };
            }
            catch (InvalidOperationException ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new BaseRegistrationResponse { Success = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new BaseRegistrationResponse { Success = false, Message = "Der opstod en uventet systemfejl." };
            }
        }
    }
}
