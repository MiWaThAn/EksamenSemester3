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
    public class GetActiveWorkLogQueryHandler : IRequestHandler<GetActiveWorkLogQuery, WorkLogDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetActiveWorkLogQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<WorkLogDto?> Handle(GetActiveWorkLogQuery request, CancellationToken cancellationToken)
        {
            var account = await _unitOfWork.Accounts.GetByIdAsync(request.accountId);
            if (account == null) return null;
            if (account.EmployeeId == null) return null;
            var emp = await _unitOfWork.Employees.GetByIdAsync(account.EmployeeId.Value);
            var activeWorkLog = await _unitOfWork.WorkLogs.GetActiveWorkLogAsNoTrackingAsync(account.EmployeeId.Value);
            if (activeWorkLog == null)
                return null;
            var projectIds = activeWorkLog.Registrations.Select(r => r.ProjectId).Distinct().ToList();
            var activityIds = activeWorkLog.Registrations.Where(r => r.ProjectActivityId.HasValue).Select(r => r.ProjectActivityId!.Value).Distinct().ToList();
            var expenseIds = activeWorkLog.Registrations.OfType<ExpenseRegistration>().Select(r => r.ExpenseId).Distinct().ToList();

            var activities = await _unitOfWork.ProjectActivities.GetQueryable().Include(a => a.Activity).Where(a => activityIds.Contains(a.Id)).ToListAsync();
            List<Project> projects = await _unitOfWork.Projects.GetQueryable().Where(p => projectIds.Contains(p.Id)).ToListAsync();
            List<Expense> expenses = await _unitOfWork.Expenses.GetQueryable().Where(e => expenseIds.Contains(e.Id)).ToListAsync();

            var dto = activeWorkLog.ToDto(projects, activities, expenses,emp.Name);

            return dto;
        }
    }
}
