using Domain.Entity.Item;
using Domain.Entity.Item.Activities;
using Domain.Entity.Item.Registrations;
using Shared.Item.Registrations.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Requests.WorkLogHandlers
{
    public static class WorkLogMapper
    {
        public static WorkLogDto ToDto(
            this WorkLog workLog,
            IEnumerable<Project> projects,
            IEnumerable<ProjectActivity> activities,
            IEnumerable<Expense> expenses) // Replace 'Expense' with your actual expense domain class
        {
            if (workLog == null) return null!;

            var timeRegistrations = workLog.Registrations
                .OfType<HourRegistration>()
                .Select(r => r.ToDto(projects, activities))
                .ToList();

            var expenseRegistrations = workLog.Registrations
                .OfType<ExpenseRegistration>()
                .Select(e => e.ToDto(projects, activities, expenses))
                .ToList();

            return new WorkLogDto(
                Id: workLog.Id,
                AccountId: workLog.EmployeeId,
                Status: workLog.Status.ToString(),
                ClockInTime: workLog.DateCreated,
                ClockOutTime: workLog.IsClosed ? workLog.DateClosed : null,
                RejectionReason: workLog.RejectionReason,
                TimeRegistrations: timeRegistrations,
                ExpenseRegistrations: expenseRegistrations
            );
        }

        public static TimeRegistrationDto ToDto(
            this HourRegistration registration,
            IEnumerable<Project> projects,
            IEnumerable<ProjectActivity> activities)
        {
            if (registration == null) return null!;

            var intervals = registration.Intervals?
                .Select(ToDto)
                .ToList() ?? new List<TimeIntervalDto>();

            // Find matching objects directly from the passed-in parameters
            var project = projects.FirstOrDefault(p => p.Id == registration.ProjectId);
            var activity = registration.ProjectActivityId.HasValue
                ? activities.FirstOrDefault(a => a.Id == registration.ProjectActivityId.Value)
                : null;

            return new TimeRegistrationDto(
                Id: registration.Id,
                ProjectId: registration.ProjectId,
                ProjectName: project?.Name ?? "Ukendt Projekt",
                ProjectActivityId: registration.ProjectActivityId,
                ProjectActivityName: activity?.Activity.Name,
                Description: registration.Description,
                Status: registration.Status.ToString(),
                IsFinished: registration.IsFinished,
                Intervals: intervals
            );
        }

        public static TimeIntervalDto ToDto(this TimeInterval interval)
        {
            if (interval == null) return null!;

            return new TimeIntervalDto(
                Id: interval.Id,
                StartTime: interval.StartTime,
                EndTime: interval.EndTime,
                Type: interval.Type.ToString(),
                IsBreak: interval.Type == TimeType.Break
            );
        }

        public static ExpenseRegistrationDto ToDto(
            this ExpenseRegistration expense,
            IEnumerable<Project> projects,
            IEnumerable<ProjectActivity> activities,
            IEnumerable<Expense> expenses)
        {
            if (expense == null) return null!;

            var project = projects.FirstOrDefault(p => p.Id == expense.ProjectId);
            var expenseType = expenses.FirstOrDefault(e => e.Id == expense.ExpenseId);
            var activity = expense.ProjectActivityId.HasValue
                ? activities.FirstOrDefault(a => a.Id == expense.ProjectActivityId.Value)
                : null;

            return new ExpenseRegistrationDto(
                Id: expense.Id,
                ProjectId: expense.ProjectId,
                ProjectName: project?.Name ?? "Ukendt Projekt",
                ProjectActivityId: expense.ProjectActivityId,
                ProjectActivityName: activity?.Activity.Name,
                Description: expense.Description,
                Status: expense.Status.ToString(),
                ExpenseId: expense.ExpenseId,
                ExpenseName: expenseType?.Name ?? "Ukendt Udgift",
                Amount: expense.Amount,
                Date: expense.Date
            );
        }
    }
}
