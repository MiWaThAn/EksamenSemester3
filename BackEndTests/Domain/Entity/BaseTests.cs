using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackEndTests.Domain.Entity
{
    #region Test Stub for Abstract Verification

    // Concrete mock class inheriting from Base to allow testing of protected/abstract logic
    public class TestEntity : Base
    {
        public TestEntity() : base()
        {
        }
    }

    #endregion

    public class BaseTests
    {
        #region Initialization Lifecycle Tests

        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            // Act
            var entity = new TestEntity();

            // Assert
            Assert.NotEqual(Guid.Empty, entity.Id);
            Assert.False(entity.IsDeleted);
            Assert.Null(entity.DeletedAt);

            // Verify timestamps are assigned and identical on initial tracking creation
            Assert.NotEqual(default, entity.CreatedAt);
            Assert.Equal(entity.CreatedAt, entity.UpdatedAt);
        }

        #endregion

        #region Soft Delete State Machine Tests

        [Fact]
        public void SoftDelete_ShouldToggleDeletionFlagsAndRecordTimestamp()
        {
            // Arrange
            var entity = new TestEntity();

            // Act
            entity.SoftDelete();

            // Assert
            Assert.True(entity.IsDeleted);
            Assert.NotNull(entity.DeletedAt);
            Assert.True(entity.DeletedAt <= DateTime.UtcNow);
        }

        [Fact]
        public void UndoSoftDelete_WhenEntityIsSoftDeleted_ShouldRestoreStateFlags()
        {
            // Arrange
            var entity = new TestEntity();
            entity.SoftDelete();
            Assert.True(entity.IsDeleted); // Guard check before reversing

            // Act
            entity.UndoSoftDelete();

            // Assert
            Assert.False(entity.IsDeleted);
            Assert.Null(entity.DeletedAt);
        }

        #endregion

        #region Concurrency Data Mechanics

        [Fact]
        public void RowVersion_ShouldAcceptAndHoldBytePayload()
        {
            // Arrange
            var entity = new TestEntity();
            byte[] mockDatabaseRowVersion = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7A, 0x15 };

            // Act
            entity.RowVersion = mockDatabaseRowVersion;

            // Assert
            Assert.NotNull(entity.RowVersion);
            Assert.Equal(8, entity.RowVersion.Length);
            Assert.Equal(mockDatabaseRowVersion, entity.RowVersion);
        }

        #endregion
    }
}
