using Domain.Entity.Item.Registrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackEndTests.Domain.Entity.Item.Registrations.HourRegistrationsTests
{
    public class HourRegistrationMathAndSlicingTests
    {
        private readonly Guid _validProjectId = Guid.NewGuid();
        private readonly WorkLog _stubWorkLog = new WorkLog { Id = Guid.NewGuid(), EmployeeId = Guid.NewGuid() };

        [Fact]
        public void TotalHours_ShouldOnlyCalculateFinishedWorkIntervals()
        {
            var baseTime = DateTime.UtcNow.Date.AddHours(8);

            //initializing with an 8:00 - 10:00 work block (2 timer)
            var reg = new HourRegistration(_validProjectId, _stubWorkLog, null, baseTime, baseTime.AddHours(2), RegistrationStatus.Pending, "Calculation");

            //manual injection of extra intervals using the internal method: 
            //10:00 - 11:00 break (1 timer)
            //11:00 - 14:00 work (3 timer)
            var technicalIntervals = new List<TimeInterval>
            {
                new TimeInterval(baseTime.AddHours(2), baseTime.AddHours(3), TimeType.Break),
                new TimeInterval(baseTime.AddHours(3), baseTime.AddHours(6), TimeType.Work)
            };
            reg.AddIntervals(technicalIntervals);

            //2 timer + 3 timer = 5 
            Assert.Equal(5.0, reg.TotalHours());
        }

        [Fact]
        public void TrimAndExtractAfter_WhenOverlapSplitsIntervalInHalf_ShouldShortenOriginalAndReturnTrailingSplit()
        {
            //10:00 - 16:00 (6 timer)
            var baseTime = DateTime.UtcNow.Date.AddHours(10);
            var reg = new HourRegistration(_validProjectId, _stubWorkLog, null, baseTime, baseTime.AddHours(6), RegistrationStatus.Pending, "Slicing Context");

            //overlap boundary cuts out the middle 12:00 - 14:00
            var overlapStart = baseTime.AddHours(2); //12:00
            var overlapEnd = baseTime.AddHours(4);   //14:00

            // Act
            List<TimeInterval> extracted = reg.TrimAndExtractAfter(overlapStart, overlapEnd);

            //assert
            //check original interval got capped to starting segment (10:00 - 12:00)
            //(i think)
            Assert.Equal(overlapStart, reg.Intervals.First().EndTime);

            //check that the remaining chunk (14:00 - 16:00)
            Assert.Single(extracted);
            Assert.Equal(overlapEnd, extracted.First().StartTime);
            Assert.Equal(baseTime.AddHours(6), extracted.First().EndTime);
        }
    }
}
