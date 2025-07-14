using HR.LeaveManagement.Application.Contracts.Identity;
using HR.LeaveManagement.Domain;
using HR.LeaveManagement.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shouldly;

namespace HR.LeaveManagement.Persistence.IntegrationTests
{
    public class HrDatabaseContextTests
    {
        private readonly HrDatabaseContext _hrDatabaseContext;
        private readonly Mock<IUserService> _userServiceMock;

        public HrDatabaseContextTests()
        {
            // Create mock for IUserService
            _userServiceMock = new Mock<IUserService>();
            _userServiceMock.Setup(x => x.UserId).Returns("test-user-id");
            _userServiceMock.Setup(x => x.UserId).Returns("test-user-id");

            var dbOptions = new DbContextOptionsBuilder<HrDatabaseContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            // Pass mock to context constructor
            _hrDatabaseContext = new HrDatabaseContext(dbOptions, _userServiceMock.Object);
        }

        [Fact]
        public async Task Save_SetDateCreatedAndModifiedValues()
        {
            // Arrange
            var leaveType = new LeaveType
            {
                Id = 1,
                DefaultDays = 10,
                Name = "Test Vacation"
            };

            // Act
            await _hrDatabaseContext.LeaveTypes.AddAsync(leaveType);
            await _hrDatabaseContext.SaveChangesAsync();

            // Assert
            leaveType.DateCreated.ShouldNotBeNull();
            leaveType.DateModified.ShouldNotBeNull();
            leaveType.CreatedBy.ShouldBe("test-user-id");
            leaveType.ModifiedBy.ShouldBe("test-user-id");
        }

        [Fact]
        public async Task Update_SetDateModifiedValue()
        {
            // Arrange - Add entity first
            var leaveType = new LeaveType
            {
                Id = 1,
                DefaultDays = 10,
                Name = "Test Vacation"
            };
            await _hrDatabaseContext.LeaveTypes.AddAsync(leaveType);
            await _hrDatabaseContext.SaveChangesAsync();

            // Reset change tracker
            _hrDatabaseContext.ChangeTracker.Clear();

            // Act - Modify entity
            leaveType.Name = "Updated Vacation";
            _hrDatabaseContext.LeaveTypes.Update(leaveType);
            await _hrDatabaseContext.SaveChangesAsync();

            // Assert
            leaveType.DateModified.ShouldNotBeNull();
            leaveType.DateCreated.ShouldNotBeNull();
            leaveType.ModifiedBy.ShouldBe("test-user-id");
            
            // DateCreated should remain unchanged
            leaveType.DateCreated.ShouldBe(leaveType.DateCreated);
        }
    }
}