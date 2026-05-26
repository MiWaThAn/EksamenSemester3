using Domain.Entity.Item.Registrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackEndTests.Domain.Entity.Item.Registrations.HourRegistrationsTests
{
    public class HourRegistrationTests
    {
        private readonly Guid _validProjectId = Guid.NewGuid();
        private readonly WorkLog _stubWorkLog = new WorkLog { Id = Guid.NewGuid(), EmployeeId = Guid.NewGuid() };

        [Fact]
        public void ValidateAgainst_WhenOverlappingRegistrationExists_ShouldThrowArgumentException()
        {
            //record from 12:00 to 14:00
            var baseTime = DateTime.UtcNow.Date.AddHours(12);
            var registration = new HourRegistration(_validProjectId, _stubWorkLog, null, baseTime, baseTime.AddHours(2), RegistrationStatus.Pending, "Task A");

            //overlapping record 13:00 to 15:00
            var overlappingRegistration = new HourRegistration(_validProjectId, _stubWorkLog, null, baseTime.AddHours(1), baseTime.AddHours(3), RegistrationStatus.Pending, "Task B");
            var existingList = new List<Registration> { overlappingRegistration };

            Assert.Throws<ArgumentException>(() => registration.ValidateAgainst(existingList));
        }

        [Fact]
        public void EndWork_WhenActiveIntervalExists_ShouldSetEndTimeToNow()
        {
            var startTime = DateTime.UtcNow.AddHours(-1);
            var reg = new HourRegistration(_validProjectId, _stubWorkLog, null, startTime, null, RegistrationStatus.Pending, "Active Work");

            reg.EndWork();

            Assert.True(reg.IsFinished);
            Assert.NotNull(reg.EndTime);
        }

        [Fact]
        public void EndWork_WhenNoActiveInterval_ShouldThrowInvalidOperationException()
        {
            var startTime = DateTime.UtcNow.AddHours(-2);
            var endTime = DateTime.UtcNow.AddHours(-1);
            var reg = new HourRegistration(_validProjectId, _stubWorkLog, null, startTime, endTime, RegistrationStatus.Pending, "Finished Work");

            Assert.Throws<InvalidOperationException>(() => reg.EndWork());
        }

        [Fact]
        public void TakeBreak_WhenWorking_ShouldCloseActiveIntervalAndOpenBreak()
        {
            var startTime = DateTime.UtcNow.AddHours(-1);
            var reg = new HourRegistration(_validProjectId, _stubWorkLog, null, startTime, null, RegistrationStatus.Pending, "Working");

            reg.TakeBreak();

            Assert.Equal(2, reg.Intervals.Count);
            Assert.NotNull(reg.Intervals.First().EndTime); //first work block ended
            Assert.Null(reg.Intervals.Last().EndTime);    //break block is active
            Assert.Equal(TimeType.Break, reg.Intervals.Last().Type);
        }

        [Fact]
        public void ResumeWork_WhenOnBreak_ShouldCloseBreakAndOpenWork()
        {
            var startTime = DateTime.UtcNow.AddHours(-2);
            var reg = new HourRegistration(_validProjectId, _stubWorkLog, null, startTime, null, RegistrationStatus.Pending, "Working");
            reg.TakeBreak(); //open break

            reg.ResumeWork();

            Assert.Equal(3, reg.Intervals.Count);
            Assert.NotNull(reg.Intervals.ElementAt(1).EndTime); //break closed
            Assert.Null(reg.Intervals.Last().EndTime);          //work reopened
            Assert.Equal(TimeType.Work, reg.Intervals.Last().Type);
        }
    }
}
