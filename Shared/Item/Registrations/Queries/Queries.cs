using MediatR;
using Shared.Item.Registrations.DTOs;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Item.Registrations.Queries
{
    public record GetActiveWorkLogQuery(Guid accountId) : IRequest<WorkLogDto>;
    public record GetWorkLogHistoryQuery(Guid AccountId) : IRequest<IEnumerable<WorkLogDto>>;
    public record GetWorkLogByIdQuery(Guid LogId) : IRequest<WorkLogDto>;
    public record GetPendingWorkLogsQuery(Guid accountId) : IRequest<IEnumerable<WorkLogDto>>;
}
