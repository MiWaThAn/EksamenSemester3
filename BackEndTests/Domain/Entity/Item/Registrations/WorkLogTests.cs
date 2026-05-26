using Domain.Builders.Item.Registration;
using Domain.Entity.Item;
using Domain.Entity.Item.Activities;
using Domain.Entity.Item.Registrations;
using Domain.Entity.Person;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackEndTests.Domain.Entity.Item.Registrations
{
    public class WorkLogTests
    {
        private readonly Employee _owningEmployee;
        private readonly Employee _unauthorizedEmployee;
        private readonly Company _stubCompany;
        private readonly Project _stubProject;
        private readonly ProjectActivity _stubActivity;

        public WorkLogTests()
        {
            // Set up standardized domain objects
            _owningEmployee = new Employee { Id = Guid.NewGuid() };
            _unauthorizedEmployee = new Employee { Id = Guid.NewGuid() };
            _stubCompany = new Company { Id = Guid.NewGuid() };
            _stubProject = new Project { Id = Guid.NewGuid() };
            _stubActivity = new ProjectActivity { Id = Guid.NewGuid() };
        }

        #region Helper Methods for State Manipulation

        private WorkLog CreateWorkLogInstance()
        {
            // Use internal constructor via reflection or direct instantiation if visible
            var log = (WorkLog)Activator.CreateInstance(typeof(WorkLog), nonPublic: true);
            log.EmployeeId = _owningEmployee.Id;
            log.DateCreated = DateTime.UtcNow;
            log.Status = ApprovalStatus.Draft;
            return log;
        }

        private void SetStatusReflection(WorkLog log, ApprovalStatus status)
        {
            var prop = typeof(WorkLog).GetProperty("Status");
            prop?.SetValue(log, status);
        }

        #endregion

        #region Guard & Security Validation Tests

        [Fact]
        public void ActionMethods_WithUnauthorizedEmployee_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var log = CreateWorkLogInstance();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => log.StartWork(_stubProject, _stubActivity, _unauthorizedEmployee));
            Assert.Throws<InvalidOperationException>(() => log.TakeBreak(_unauthorizedEmployee));
            Assert.Throws<InvalidOperationException>(() => log.ResumeWork(_unauthorizedEmployee));
            Assert.Throws<InvalidOperationException>(() => log.EndWork(_unauthorizedEmployee));
            Assert.Throws<InvalidOperationException>(() => log.ClockOut(_unauthorizedEmployee));
        }

        [Fact]
        public void Modification_WhenLogIsApproved_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var log = CreateWorkLogInstance();
            SetStatusReflection(log, ApprovalStatus.Approved);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => log.StartWork(_stubProject, _stubActivity, _owningEmployee));
            Assert.Throws<InvalidOperationException>(() => log.TakeBreak(_owningEmployee));
            Assert.Throws<InvalidOperationException>(() => log.SwitchActivity(_stubActivity, _owningEmployee));
        }

        [Fact]
        public void StartWork_WhenAlreadyHasActiveRegistration_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var log = CreateWorkLogInstance();
            log.StartWork(_stubProject, _stubActivity, _owningEmployee);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => log.StartWork(_stubProject, _stubActivity, _owningEmployee));
        }

        #endregion

        #region State Machine Workflow Tests

        [Fact]
        public void Workflow_StartWorkThenClockOut_ShouldProperlyCloseActiveIntervals()
        {
            // Arrange
            var log = CreateWorkLogInstance();

            // Act - Start working
            log.StartWork(_stubProject, _stubActivity, _owningEmployee);
            Assert.True(log.HasActiveRegistration);
            Assert.NotNull(log.ActiveRegistrationId);

            // Act - Clock out
            log.ClockOut(_owningEmployee);

            // Assert
            Assert.True(log.IsClosed);
            Assert.Null(log.ActiveRegistrationId);
            Assert.False(log.HasActiveRegistration);
            Assert.NotNull(log.DateClosed);
        }

        [Fact]
        public void SubmitForApproval_FromDraftState_ShouldTransitionToPending()
        {
            // Arrange
            var log = CreateWorkLogInstance();

            // Act
            log.SubmitForApproval(_owningEmployee);

            // Assert
            Assert.Equal(ApprovalStatus.Pending, log.Status);
        }

        [Fact]
        public void ReviewProcess_RejectThenApprove_ShouldManageChildRegistrations()
        {
            // Arrange
            var log = CreateWorkLogInstance();
            log.StartWork(_stubProject, _stubActivity, _owningEmployee);
            log.EndWork(_owningEmployee);

            SetStatusReflection(log, ApprovalStatus.Pending);

            // Act - Reject log
            log.Reject(_stubCompany, "Missing descriptions");
            Assert.Equal(ApprovalStatus.Rejected, log.Status);
            Assert.Equal("Missing descriptions", log.RejectionReason);
            Assert.All(log.Registrations, r => Assert.Equal(RegistrationStatus.Afvist, r.Status));

            // Act - Resubmitting to Draft/Pending resets child allocations
            SetStatusReflection(log, ApprovalStatus.Draft);
            log.SubmitForApproval(_owningEmployee);
            SetStatusReflection(log, ApprovalStatus.Pending);

            // Act - Approve log
            log.Approve(_stubCompany);
            Assert.Equal(ApprovalStatus.Approved, log.Status);
            Assert.All(log.Registrations, r => Assert.Equal(RegistrationStatus.Godkendt, r.Status));
        }

        #endregion

        #region Overlap Adjustment Calculations

        [Fact]
        public void CreateRegistration_WithOverlappingManualRecord_ShouldTriggerOverlapSlicingLogic()
        {
            // Arrange
            var log = CreateWorkLogInstance();
            var baseTime = DateTime.UtcNow.Date.AddHours(10); // 10:00 AM

            // 1. Setup an existing registration covering 10:00 - 14:00 (4 hours)
            var existingBuilder = new HourRegistrationBuilder()
                .WithProject(_stubProject)
                .WithProjectActivity(_stubActivity)
                .WithStart(baseTime)
                .WithEnd(baseTime.AddHours(4))
                .WithStatus(RegistrationStatus.Pending);

            log.CreateRegistration(existingBuilder);

            // 2. Inject an overlapping manual entry into the center: 11:00 - 12:00 (1 hour)
            var manualOverlapBuilder = new HourRegistrationBuilder()
                .WithProject(_stubProject)
                .WithProjectActivity(_stubActivity)
                .WithStart(baseTime.AddHours(1))
                .WithEnd(baseTime.AddHours(2))
                .WithStatus(RegistrationStatus.Pending);

            // Act
            log.CreateRegistration(manualOverlapBuilder);

            // Assert
            // The middle overlap splits the original into 2 items (Pre-overlap chunk & Post-overlap chunk)
            // Plus the new manual overlap record itself = 3 registrations total.
            var activeRegs = log.Registrations.Cast<HourRegistration>().Where(r => !r.IsDeleted).ToList();

            Assert.Equal(3, activeRegs.Count);

            // Validate mathematical integrity of calculated work blocks
            // Total should equal original bounds (4 hours) because it was simply fragmented, not deleted.
            double aggregateHours = activeRegs.Sum(r => r.TotalHours());
            Assert.Equal(4.0, aggregateHours);
        }

        #endregion
    }
}
