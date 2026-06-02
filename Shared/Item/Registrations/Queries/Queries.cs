using MediatR;
using Shared.Item.Registrations.DTOs;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Item.Registrations.Queries
{
    public record GetActiveWorkLogQuery(Guid employeeId) : IRequest<WorkLogDto>;
    public record GetWorkLogHistoryQuery(Guid employeeId) : IRequest<IEnumerable<WorkLogDto>>;
    public record GetWorkLogByIdQuery(Guid LogId) : IRequest<WorkLogDto>;
    public record GetPendingWorkLogsQuery(Guid companyId) : IRequest<IEnumerable<WorkLogDto>>;
}
