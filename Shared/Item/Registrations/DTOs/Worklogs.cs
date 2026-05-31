using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Item.Registrations.DTOs
{
    // --- FÆLLES BASE RECORD ---
    public abstract record RegistrationDto(
        Guid Id,
        Guid ProjectId,
        string ProjectName,
        Guid? ProjectActivityId,
        string? ProjectActivityName,
        string Description,
        string Status // "Pending", "Godkendt", "Afvist"
    );

    // --- TIMEREGISTRERING (Nedarver fra RegistrationDto) ---
    public record TimeRegistrationDto(
        Guid Id,
        Guid ProjectId,
        string ProjectName,
        Guid? ProjectActivityId,
        string? ProjectActivityName,
        string Description,
        string Status,
        bool IsFinished,
        List<TimeIntervalDto> Intervals
    ) : RegistrationDto(Id, ProjectId, ProjectName, ProjectActivityId, ProjectActivityName, Description, Status);

    // --- UDGIFTSREGISTRERING (Nedarver fra RegistrationDto) ---
    public record ExpenseRegistrationDto(
        Guid Id,
        Guid ProjectId,
        string ProjectName,
        Guid? ProjectActivityId,
        string? ProjectActivityName,
        string Description,
        string Status,
        Guid ExpenseId,
        string ExpenseName,
        decimal Amount,
        DateTime Date
    ) : RegistrationDto(Id, ProjectId, ProjectName, ProjectActivityId, ProjectActivityName, Description, Status);

    // --- HJÆLPE RECORDS ---
    public record TimeIntervalDto(
        Guid Id,
        DateTime StartTime,
        DateTime? EndTime,
        string Type, // Fx "Work" eller "Break"
        bool IsBreak
    );

    public record WorkLogDto(
        Guid Id,
        Guid AccountId,
        string EmployeeName,
        string Status,
        DateTime ClockInTime,
        DateTime? ClockOutTime,
        string? RejectionReason,
        List<TimeRegistrationDto> TimeRegistrations,
        List<ExpenseRegistrationDto> ExpenseRegistrations
    );
}
