using Application.Interfaces;
using Domain.Builders.Item;
using Domain.Builders.Item.Registration;
using Domain.Entity.Item.Activities;
using Domain.Entity.Item.Registrations;
using MediatR;
using Shared.Item.Registrations.Commands.Time;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Application.Commands.Item.Registrations.Time
{
    internal class ManualTimeRegistrationHandler : IRequestHandler<ManualTimeRegistrationCommand, BaseRegistrationResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ManualTimeRegistrationHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<BaseRegistrationResponse> Handle(ManualTimeRegistrationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, cancellationToken);
                var worklog = await _unitOfWork.WorkLogs.GetActiveByEmployeeIdAsync(request.EmployeeId, cancellationToken);
                if (worklog == null)
                    return new BaseRegistrationResponse { Success = false, Message = "Aktivt worklog ikke fundet for medarbejderen." };
                var project = await _unitOfWork.Projects.GetByIdAsync(request.ProjectId, cancellationToken);
                if (project == null)
                    return new BaseRegistrationResponse { Success = false, Message = "Projekt ikke fundet." };
                var type = request.isWork ? TimeType.Work : TimeType.Break;
                var builder = new HourRegistrationBuilder()
                    .WithProject(project)
                    .WithStart(request.StartTime)
                    .WithType(type)
                    .WithStatus(RegistrationStatus.Pending)
                    .WithDescription(request.Description);
                if (request.ProjectActivityId.HasValue)
                {
                    var activity = await _unitOfWork.ProjectActivities.GetByIdAsync(request.ProjectActivityId.Value, cancellationToken);
                    if (activity != null)
                        builder.WithProjectActivity(activity);
                }
                worklog.CreateRegistration(builder);
                await _unitOfWork.CompleteAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return new BaseRegistrationResponse { Success = true, Message = "Tidsregistrering er tilføjet." };
            }
            catch (InvalidOperationException ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new BaseRegistrationResponse { Success = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new BaseRegistrationResponse { Success = false, Message = "En fejl opstod under tidsregistreringen." };
            }
        }
    }
}
