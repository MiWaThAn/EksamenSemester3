using Application.Interfaces;
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
    public class GetWorkLogByIdQueryHandler : IRequestHandler<GetWorkLogByIdQuery, WorkLogDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetWorkLogByIdQueryHandler(IUnitOfWork unitofwork)
        {
            _unitOfWork = unitofwork;
        }

        public async Task<WorkLogDto?> Handle(GetWorkLogByIdQuery request, CancellationToken cancellationToken)
        {
            var worklog = await _unitOfWork.WorkLogs.GetQueryable().Include(w=>w.Registrations).AsNoTracking().FirstOrDefaultAsync(w => w.Id == request.LogId);
            if (worklog == null) return null;
            var projectIds = worklog.Registrations.Select(r => r.ProjectId).Distinct().ToList();
            var activityIds = worklog.Registrations.Where(r => r.ProjectActivityId.HasValue).Select(r => r.ProjectActivityId!.Value).Distinct().ToList();
            var expenseIds = worklog.Registrations.OfType<ExpenseRegistration>().Select(r => r.ExpenseId).Distinct().ToList();

            var activities = await _unitOfWork.ProjectActivities.GetQueryable().Include(a => a.Activity).Where(a => activityIds.Contains(a.Id)).ToListAsync();
            List<Project> projects = await _unitOfWork.Projects.GetQueryable().Where(p => projectIds.Contains(p.Id)).ToListAsync();
            List<Expense> expenses = await _unitOfWork.Expenses.GetQueryable().Where(e => expenseIds.Contains(e.Id)).ToListAsync();
            var dto = worklog.ToDto(projects, activities, expenses);

            return dto;
        }
    }
}
