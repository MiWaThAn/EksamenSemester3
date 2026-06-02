using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Requests
{
    public sealed record StartWorkRequest(
        Guid ProjectId,
        Guid ProjectActivityId);
}
