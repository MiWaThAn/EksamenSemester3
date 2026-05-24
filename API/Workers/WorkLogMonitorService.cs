using Application.Commands.Notification;
using Application.Interfaces;
using Domain.Entity.Item.Registrations;
using Domain.Interfaces.Notification;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Workers
{
    public class WorkLogMonitorService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public WorkLogMonitorService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    var activeWorkLogs = await unitOfWork.WorkLogs.GetAllActiveWorkLogsAsync(stoppingToken);

                    foreach (var log in activeWorkLogs)
                    {
                        var accountId = await unitOfWork.Employees.GetAccountIdAsync(log.EmployeeId, stoppingToken);
                        if(accountId == null) continue;
                        var totalHoursWorkedToday = log.CalculateHoursSinceLastBreak(); 

                        if (log.HasActiveRegistration && totalHoursWorkedToday >= 6.0)
                        {
                            if (log.LastRemindedAt <= DateTime.UtcNow.AddMinutes(-5))
                            {
                                log.Remind();
                                await mediator.Send(new NotifyUserCommand(
                                    accountId.Value,
                                    "Husk at holde pause!",
                                    $"Du har nu arbejdet i {Math.Round(totalHoursWorkedToday, 1)} timer i dag uden pause."
                                ), stoppingToken);
                            }
                        }
                        if (!log.HasActiveRegistration && log.LastActivityEndTime <= DateTime.UtcNow.AddHours(-1))
                        {
                            if (log.LastRemindedAt == null || log.LastRemindedAt <= DateTime.UtcNow.AddHours(-1))
                            {
                                log.Remind();

                                await mediator.Send(new NotifyUserCommand(
                                    accountId.Value,
                                    "Glemt at stemple ud?",
                                    "Det er over en time siden din seneste opgave stoppede. Husk at lukke din arbejdsdag."
                                ), stoppingToken);
                            }
                        }
                    }

                    await unitOfWork.CompleteAsync(stoppingToken);
                }
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
