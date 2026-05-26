using Application.Interfaces;
using Application.Requests.WorkLogHandlers;
using Domain.Entity.Item;
using Domain.Entity.Item.Registrations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Item.Registrations.DTOs;
using Shared.Item.Registrations.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Requests.WorkLogHandlers
{
    public class GetWorkLogHistoryQueryHandler : IRequestHandler<GetWorkLogHistoryQuery, IEnumerable<WorkLogDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetWorkLogHistoryQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<WorkLogDto>> Handle(GetWorkLogHistoryQuery request, CancellationToken cancellationToken)
        {
            var account = await _unitOfWork.Accounts.GetByIdAsync(request.AccountId, cancellationToken);
            if (account == null || !account.EmployeeId.HasValue)
                return Enumerable.Empty<WorkLogDto>();

            var employeeId = account.EmployeeId.Value;

            var worklogs = await _unitOfWork.WorkLogs.GetQueryable()
                .Include(w => w.Registrations)
                .AsNoTracking()
                .Where(w => w.EmployeeId == employeeId)
                .OrderByDescending(w => w.DateCreated)
                .ToListAsync(cancellationToken);

            if (worklogs == null || !worklogs.Any())
                return Enumerable.Empty<WorkLogDto>();

            var allProjectIds = worklogs.SelectMany(w => w.Registrations).Select(r => r.ProjectId).Distinct().ToList();

            var allActivityIds = worklogs.SelectMany(w => w.Registrations)
                .Where(r => r.ProjectActivityId.HasValue)
                .Select(r => r.ProjectActivityId!.Value)
                .Distinct().ToList();

            var allExpenseIds = worklogs.SelectMany(w => w.Registrations)
                .OfType<ExpenseRegistration>()
                .Select(r => r.ExpenseId)
                .Distinct().ToList();


            var globalActivities = await _unitOfWork.ProjectActivities.GetQueryable()
                .Include(a => a.Activity)
                .Where(a => allActivityIds.Contains(a.Id))
                .ToListAsync(cancellationToken);

            var globalProjects = await _unitOfWork.Projects.GetQueryable()
                .Where(p => allProjectIds.Contains(p.Id))
                .ToListAsync(cancellationToken);

            var globalExpenses = await _unitOfWork.Expenses.GetQueryable()
                .Where(e => allExpenseIds.Contains(e.Id))
                .ToListAsync(cancellationToken);

            List<WorkLogDto> workLogDtos = new();
            foreach (WorkLog worklog in worklogs)
            {

                var logProjectIds = worklog.Registrations.Select(r => r.ProjectId).ToHashSet();
                var logActivityIds = worklog.Registrations.Where(r => r.ProjectActivityId.HasValue).Select(r => r.ProjectActivityId!.Value).ToHashSet();
                var logExpenseIds = worklog.Registrations.OfType<ExpenseRegistration>().Select(r => r.ExpenseId).ToHashSet();


                var projects = globalProjects.Where(p => logProjectIds.Contains(p.Id)).ToList();
                var activities = globalActivities.Where(a => logActivityIds.Contains(a.Id)).ToList();
                var expenses = globalExpenses.Where(e => logExpenseIds.Contains(e.Id)).ToList();

                var dto = worklog.ToDto(projects, activities, expenses);
                workLogDtos.Add(dto);
            }

            return workLogDtos;
        }
    }
}
