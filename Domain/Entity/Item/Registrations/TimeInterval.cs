using Domain.Guards;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain.Entity.Item.Registrations
{
    [ComplexTypeAttribute()]
    public class TimeInterval
    {
        public DateTime StartTime { get; internal set; }
        public DateTime? EndTime { get; internal set; }
        public TimeType Type { get; internal set; }
        public TimeInterval(DateTime startTime, DateTime? endTime, TimeType type)
        {
            StartTime = startTime;
            EndTime = endTime;
            Type = type;
        }
        public void UpdateRange(DateTime start, DateTime? end)
        {
            StartTime = start;
            EndTime = end;
        }
        public void SetEndTime(DateTime endTime)
        {
            Guard.AgainstInvalidTimeRange(StartTime, endTime);
            Guard.AgainstNull(endTime, nameof(endTime));
            EndTime = endTime;
        }
    }
}
