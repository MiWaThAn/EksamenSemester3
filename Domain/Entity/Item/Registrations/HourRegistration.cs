using Domain.Builders.Item.Registration;
using Domain.Guards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain.Entity.Item.Registrations
{
    public class HourRegistration : Registration
    {
        public DateTime StartTime => _intervals.Count > 0 ? _intervals.Min(i => i.StartTime) : DateTime.MinValue;
        public DateTime? EndTime => _intervals.Count > 0 ? _intervals.Where(i => i.EndTime.HasValue).Max(i => i.EndTime) : null;
        public bool IsFinished = false;
        private readonly List<TimeInterval> _intervals = new();
        public IReadOnlyCollection<TimeInterval> Intervals => _intervals.AsReadOnly();

        public HourRegistration() { }

        internal HourRegistration(Guid ProjectId, WorkLog workLog, Guid? activityId, DateTime startTime, DateTime? endTime, RegistrationStatus status, string description): base(ProjectId, workLog, activityId, description, status)
        {
            _intervals.Add(new TimeInterval(startTime, endTime, TimeType.Work));
        }
        internal HourRegistration(Guid ProjectId, WorkLog workLog, Guid? activityId, DateTime startTime, DateTime? endTime, TimeType timeType, RegistrationStatus status, string description) : base(ProjectId, workLog, activityId, description, status) 
        {
            _intervals.Add(new TimeInterval(startTime, endTime, timeType)); 
        }
        internal override void ValidateAgainst(IEnumerable<Registration> existingRegistrations)
        {
            if (existingRegistrations.Any(r => r.Id == this.Id))
                throw new ArgumentException("Denne registrering er allerede tilføjet.");
            var otherTimes = existingRegistrations.OfType<HourRegistration>();
            if (otherTimes.Any(r => OverlapsWith(r)))
                throw new ArgumentException("Overlappende tidsregistrering fundet.");
        }

        internal void AddIntervals(IEnumerable<TimeInterval> intervals) => _intervals.AddRange(intervals);

        private bool OverlapsWith(HourRegistration other)
        {
            var thisEnd = this.EndTime ?? DateTime.UtcNow;
            var otherEnd = other.EndTime ?? DateTime.UtcNow;
            return this.StartTime < otherEnd && thisEnd > other.StartTime;
        }

        internal void EndWork()
        {
            Guard.AgainstInvalidTimeRange(StartTime, DateTime.UtcNow);
            var active = FindActive();
            if (active == null)
                throw new InvalidOperationException("Ingen aktiv tidsinterval at afslutte.");
            active.SetEndTime(DateTime.UtcNow);
            UpdatedAt = DateTime.UtcNow;
            MarkAsPending();
        }
        internal void ClockOut()
        {
            Guard.AgainstInvalidTimeRange(StartTime, DateTime.UtcNow);
            var active = FindActive();
            if (active != null)
                active.SetEndTime(DateTime.UtcNow);
            IsFinished = true;
            UpdatedAt = DateTime.UtcNow;
        }

        internal void SetEndTime(DateTime endTime)
        {
            Guard.AgainstNull(endTime, nameof(endTime));
            Guard.AgainstInvalidTimeRange(StartTime, endTime);
            var active = FindActive();
            if (active == null)
                throw new InvalidOperationException("Ingen aktiv tidsinterval at afslutte.");
            active.SetEndTime(endTime);
            UpdatedAt = DateTime.UtcNow;
            MarkAsPending();
        }

        internal void TakeBreak()
        {
            var active = FindActive();
            if (active != null)
                active.SetEndTime(DateTime.UtcNow);
            _intervals.Add(new TimeInterval(DateTime.UtcNow, null, TimeType.Break));
            UpdatedAt = DateTime.UtcNow;
            MarkAsPending();
        }
        internal void StartWork()
        {
            var activeBreak = FindActive();
            if (activeBreak != null && activeBreak.Type == TimeType.Break)
                activeBreak.SetEndTime(DateTime.UtcNow);
            if (activeBreak != null && activeBreak.Type != TimeType.Break)
                throw new InvalidOperationException("Ingen pause igangsat.");
            _intervals.Add(new TimeInterval(DateTime.UtcNow, null, TimeType.Work));
        }
        internal void ResumeWork()
        {
            StartWork();
            UpdatedAt = DateTime.UtcNow;
            MarkAsPending();
        }

        private TimeInterval? FindActive()
        {
            return _intervals.FirstOrDefault(i => !i.EndTime.HasValue);
        }

        // Fixed: Removed the redundant 'this is HourRegistration' verification check
        internal void CreateTimeInterval(DateTime start, DateTime? end, TimeType type)
        {
            var interval = new TimeInterval(start, end, type);
            _intervals.Add(interval);
            UpdatedAt = DateTime.UtcNow;
            MarkAsPending();
        }


        internal void RemoveTimeInterval(Guid intervalId)
        {
            var interval = _intervals.FirstOrDefault(i => i.Id == intervalId);
            if (interval == null)
                throw new ArgumentException("Tidsinterval ikke fundet.");
            _intervals.Remove(interval);
            UpdatedAt = DateTime.UtcNow;
            MarkAsPending();
        }

        internal void UpdateTimeInterval(Guid intervalId, DateTime? newStart, DateTime? newEnd, TimeType? newType)
        {
            var interval = _intervals.FirstOrDefault(i => i.Id == intervalId);
            if (interval == null)
                throw new ArgumentException("Tidsinterval ikke fundet.");
            if (!interval.EndTime.HasValue)
                throw new ArgumentException("Kan ikke redigerer i et aktiv tidsinterval.");

            var updatedStart = newStart ?? interval.StartTime;
            var updatedEnd = newEnd ?? interval.EndTime.Value;
            var updatedType = newType ?? interval.Type;

            Guard.AgainstInvalidTimeRange(updatedStart, updatedEnd);
            interval.UpdateRange(updatedStart, updatedEnd);
            interval.SetType(updatedType);
            UpdatedAt = DateTime.UtcNow;
            MarkAsPending();
        }

        public double HoursSinceBreak()
        {
            var active = FindActive();
            if (active == null || active.Type != TimeType.Work)
                return 0;
            var lastBreak = _intervals.LastOrDefault(i => i.Type == TimeType.Break && i.EndTime.HasValue);
            var breakEnd = lastBreak?.EndTime ?? StartTime;
            var hours = (DateTime.UtcNow - breakEnd).TotalHours;
            return (double)hours;
        }
        public bool HasActive()
        {
            var active = FindActive();
            return active != null;
        }
        public bool HasHadBreak()
        {
            return _intervals.Any(i => i.Type == TimeType.Break);
        }

        public double TotalHours()
        {
            double total = 0;
            foreach (var interval in _intervals)
            {
                if (interval.Type == TimeType.Work && interval.EndTime.HasValue)
                {
                    total += (double)(interval.EndTime.Value - interval.StartTime).TotalHours;
                }
            }
            return total;
        }

        internal List<TimeInterval> TrimAndExtractAfter(DateTime overlapStart, DateTime overlapEnd)
        {
            var extracted = new List<TimeInterval>();
            var toRemove = new List<TimeInterval>();

            foreach (var interval in _intervals)
            {
                var intervalEnd = interval.EndTime ?? DateTime.UtcNow;

                if (interval.StartTime >= overlapStart && intervalEnd <= overlapEnd)
                {
                    toRemove.Add(interval);
                }
                else if (interval.StartTime < overlapStart && intervalEnd > overlapEnd)
                {
                    var originalEnd = interval.EndTime;
                    interval.UpdateRange(interval.StartTime, overlapStart);
                    extracted.Add(new TimeInterval(overlapEnd, originalEnd, interval.Type));
                }
                else if (interval.StartTime >= overlapStart && interval.StartTime < overlapEnd && intervalEnd > overlapEnd)
                {
                    interval.UpdateRange(overlapEnd, interval.EndTime);
                }
                else if (interval.StartTime < overlapStart && intervalEnd > overlapStart && intervalEnd <= overlapEnd)
                {
                    interval.UpdateRange(interval.StartTime, overlapStart);
                }
                else if (interval.StartTime >= overlapEnd)
                {
                    extracted.Add(interval);
                    toRemove.Add(interval);
                }
            }

            foreach (var item in toRemove)
            {
                _intervals.Remove(item);
            }

            return extracted;
        }
    }
}