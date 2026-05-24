using Domain.Builders.Item.Registration;
using Domain.Guards;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity.Item.Registrations
{
    public class HourRegistration : Registration
    {
        public DateTime StartTime => _intervals.Count > 0 ? _intervals.Min(i => i.StartTime) : DateTime.MinValue;
        public DateTime? EndTime => (_intervals.Count > 0 && _intervals.All(i => i.EndTime.HasValue))
            ? _intervals.Max(i => i.EndTime)
            : null;
        public bool IsFinished => EndTime.HasValue;
        private readonly List<TimeInterval> _intervals = new();
        public IReadOnlyCollection<TimeInterval> Intervals => _intervals.AsReadOnly();
        public HourRegistration()
        {

        }
        internal HourRegistration(Guid ProjectId, WorkLog workLog, Guid? activityId, DateTime startTime, DateTime? endTime, string description, RegistrationStatus status) : base(ProjectId, workLog, activityId, description, status)
        {
            _intervals.Add(new TimeInterval(startTime, endTime, TimeType.Work));
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
            if (active == null)
                throw new InvalidOperationException("Ingen aktiv tidsinterval at tage pause fra.");
            active.SetEndTime(DateTime.UtcNow);
            _intervals.Add(new TimeInterval(DateTime.UtcNow, null, TimeType.Break));
            UpdatedAt = DateTime.UtcNow;
            MarkAsPending();
        }
        internal void ResumeWork()
        {
            var activeBreak = FindActive();
            if (activeBreak == null || activeBreak.Type != TimeType.Break)
                throw new InvalidOperationException("Ingen pause igangsat.");

            activeBreak.SetEndTime(DateTime.UtcNow);
            _intervals.Add(new TimeInterval(DateTime.UtcNow, null, TimeType.Work));
            UpdatedAt = DateTime.UtcNow;
            MarkAsPending();
        }
        private TimeInterval? FindActive()
        {
            return _intervals.FirstOrDefault(i => !i.EndTime.HasValue);
        }
        internal void CreateTimeInterval(DateTime start, DateTime? end, TimeType type)
        {
            if (this is HourRegistration hourReg)
            {
                var interval = new TimeInterval(start, end, type);
                _intervals.Add(interval);
                UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                throw new InvalidOperationException("Tidsintervaller kan kun tilføjes til tidsregistreringer.");
            }
            MarkAsPending();
        }
        //håndterer trimning og ekstraktion af tidsintervaller baseret på overlap med et givent interval (retunerer intervaller der kom efter overlappet)
        internal List<TimeInterval> TrimAndExtractAfter(DateTime overlapStart, DateTime overlapEnd)
        {
            var extracted = new List<TimeInterval>();
            var toRemove = new List<TimeInterval>();

            foreach (var interval in _intervals)
            {
                var intervalEnd = interval.EndTime ?? DateTime.UtcNow;

                //completely eaten by overlap
                if (interval.StartTime >= overlapStart && intervalEnd <= overlapEnd)
                {
                    toRemove.Add(interval);
                }
                //overlap splits interval in half
                else if (interval.StartTime < overlapStart && intervalEnd > overlapEnd)
                {
                    var originalEnd = interval.EndTime;
                    interval.UpdateRange(interval.StartTime, overlapStart);
                    extracted.Add(new TimeInterval(overlapEnd, originalEnd, interval.Type));
                }
                //overlap eats into start of interval
                else if (interval.StartTime >= overlapStart && interval.StartTime < overlapEnd && intervalEnd > overlapEnd)
                {
                    interval.UpdateRange(overlapEnd, interval.EndTime);
                }
                //overlap eats into end of interval
                else if (interval.StartTime < overlapStart && intervalEnd > overlapStart && intervalEnd <= overlapEnd)
                {
                    interval.UpdateRange(interval.StartTime, overlapStart);
                }
                //interval is after the overlap zone
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
