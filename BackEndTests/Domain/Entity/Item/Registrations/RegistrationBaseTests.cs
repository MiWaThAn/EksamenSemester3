using Domain.Entity.Item.Registrations;
using Domain.Entity.Person;
using System;
using System.Collections.Generic;
using Xunit;
using System.Text;

namespace BackEndTests.Domain.Entity.Item.Registrations
{
    public class RegistrationBaseTests
    {
        private readonly Guid _validProjectId = Guid.NewGuid();
        private readonly WorkLog _stubWorkLog = new WorkLog { Id = Guid.NewGuid(), EmployeeId = Guid.NewGuid() };
        private readonly Company _stubCompany = new Company { Id = Guid.NewGuid() };

        //¨test subclass to expose the protected base constructor
        private class TestRegistration : Registration
        {
            public TestRegistration(Guid projId, WorkLog wl, Guid? actId, string? desc, RegistrationStatus status) : base(projId, wl, actId, desc, status) { }
        }

        [Fact]
        public void BaseConstructor_ShouldInitializeCorrectly()
        {
            var reg = new TestRegistration(_validProjectId, _stubWorkLog, null, "Test desc", RegistrationStatus.Pending);

            Assert.Equal(_stubWorkLog.EmployeeId, reg.EmployeeId);
            Assert.Equal(_stubWorkLog.Id, reg.WorkLogId);
            Assert.Equal(_validProjectId, reg.ProjectId);
            Assert.Equal("Test desc", reg.Description);
            Assert.Equal(RegistrationStatus.Pending, reg.Status);
        }

        [Fact]
        public void Approve_WhenPending_ShouldChangeStatusToGodkendt()
        {
            var reg = new TestRegistration(_validProjectId, _stubWorkLog, null, null, RegistrationStatus.Pending);

            reg.Approve(_stubCompany);

            Assert.Equal(RegistrationStatus.Godkendt, reg.Status);
        }

        [Fact]
        public void Approve_WhenNotPending_ShouldThrowInvalidOperationException()
        {
            var reg = new TestRegistration(_validProjectId, _stubWorkLog, null, null, RegistrationStatus.Afvist);

            Assert.Throws<InvalidOperationException>(() => reg.Approve(_stubCompany));
        }

        [Fact]
        public void Reject_WhenPending_ShouldChangeStatusToAfvist()
        {
            var reg = new TestRegistration(_validProjectId, _stubWorkLog, null, null, RegistrationStatus.Pending);

            reg.Reject(_stubCompany);

            Assert.Equal(RegistrationStatus.Afvist, reg.Status);
        }

        [Fact]
        public void LinkToActivity_ShouldSetActivityAndResetToPendingIfPreviouslyAfvist()
        {
            var reg = new TestRegistration(_validProjectId, _stubWorkLog, null, null, RegistrationStatus.Afvist);
            var newActivityId = Guid.NewGuid();

            reg.LinkToActivity(newActivityId);

            Assert.Equal(newActivityId, reg.ProjectActivityId);
            Assert.Equal(RegistrationStatus.Pending, reg.Status);
        }

        [Fact]
        public void UnlinkFromActivity_ShouldNullifyActivityId()
        {
            var activityId = Guid.NewGuid();
            var reg = new TestRegistration(_validProjectId, _stubWorkLog, activityId, null, RegistrationStatus.Pending);

            reg.UnlinkFromActivity();

            Assert.Null(reg.ProjectActivityId);
        }
    }
}
