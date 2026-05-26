using Domain.Entity.Item.Activities;
using System;
using System.Collections.Generic;
using System.Text;
namespace BackEndTests.Domain.Entity.Item.Activities
{
    public class ActivityTests
    {
        private readonly Guid _validCompanyId = Guid.NewGuid();
        private readonly string _validName = "Udgravning";
        private readonly string _validDescription = "Udgravning til rørlægning.";

        //constructor tests

        [Fact]
        public void Constructor_WithValidParameters_ShouldInitializeCorrectly()
        {
            var activity = new Activity(_validName, _validDescription, _validCompanyId);

            Assert.Equal(_validName, activity.Name);
            Assert.Equal(_validDescription, activity.Description);
            Assert.Equal(_validCompanyId, activity.CompanyId);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Constructor_WhenNameIsNullOrEmpty_ShouldThrowException(string invalidName)
        {
            Assert.Throws<ArgumentException>(() => new Activity(invalidName, _validDescription, _validCompanyId));
        }

        [Fact]
        public void Constructor_WhenDescriptionIsNull_ShouldThrowException()
        {
            Assert.Throws<ArgumentNullException>(() => new Activity(_validName, null, _validCompanyId));
        }

        [Fact]
        public void Constructor_WhenCompanyIdIsEmpty_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new Activity(_validName, _validDescription, Guid.Empty));
        }

        //UpdateActivityName tests

        [Fact]
        public void UpdateActivityName_WithValidName_ShouldUpdateNameAndTimestamp()
        {
            var activity = new Activity(_validName, _validDescription, _validCompanyId);
            var newName = "Updated Safety Drill";
            var beforeUpdate = DateTime.UtcNow;

            activity.UpdateActivityName(newName);

            Assert.Equal(newName, activity.Name);
            Assert.True(activity.UpdatedAt >= beforeUpdate, "UpdatedAt timestamp should be updated to current time.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void UpdateActivityName_WhenNameIsNullOrEmpty_ShouldThrowException(string invalidName)
        {
            var activity = new Activity(_validName, _validDescription, _validCompanyId);

            Assert.Throws<ArgumentException>(() => activity.UpdateActivityName(invalidName));
        }
        
        //UpdateActivityDescription tests

        [Fact]
        public void UpdateActivityDescription_WithValidDescription_ShouldUpdateDescriptionAndTimestamp()
        {
           
            var activity = new Activity(_validName, _validDescription, _validCompanyId);
            var newDescription = "Updated description text.";
            var beforeUpdate = DateTime.UtcNow;

            
            activity.UpdateActivityDescription(newDescription);

            
            Assert.Equal(newDescription, activity.Description);
            Assert.True(activity.UpdatedAt >= beforeUpdate, "UpdatedAt timestamp should be updated to current time.");
        }

        [Fact]
        public void UpdateActivityDescription_WhenDescriptionIsNull_ShouldThrowException()
        {
            
            var activity = new Activity(_validName, _validDescription, _validCompanyId);

            
            Assert.Throws<ArgumentNullException>(() => activity.UpdateActivityDescription(null));
        }

    }
}
