using HR.LeaveManagement.Application.Contracts.Email;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveRequest.Commands.CancelLeaveRequest;
using HR.LeaveManagement.Application.Models.Email;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using Moq;
using Shouldly;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveRequests.Commands
{
    public class CancelLeaveRequestHandlerTests
    {
        private readonly Mock<ILeaveRequestRepository> _mockLeaveRequestRepo;
        private readonly Mock<ILeaveAllocationRepository> _mockLeaveAllocationRepo;
        private readonly Mock<IEmailSender> _mockEmailSender;

        public CancelLeaveRequestHandlerTests()
        {
            _mockLeaveRequestRepo = MockLeaveRequestRepository.GetMockLeaveRequestRepository();
            _mockLeaveAllocationRepo = new Mock<ILeaveAllocationRepository>();
            _mockEmailSender = new Mock<IEmailSender>();

            // Setup default allocation mock behavior
            _mockLeaveAllocationRepo.Setup(x =>
                    x.GetUserAllocations(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(new Domain.LeaveAllocation
                {
                    Id = 1,
                    NumberOfDays = 10
                });
        }

        [Fact]
        public async Task CancelApprovedLeaveRequest_ShouldUpdateAllocation()
        {
            // Arrange
            // Get and modify an existing request to be approved
            var approvedRequest = await _mockLeaveRequestRepo.Object.GetByIdAsync(1);
            approvedRequest.Approved = true;
            await _mockLeaveRequestRepo.Object.UpdateAsync(approvedRequest);

            var handler = new CancelLeaveRequestCommandHandler(
                _mockLeaveRequestRepo.Object,
                _mockLeaveAllocationRepo.Object,
                _mockEmailSender.Object);

            // Act
            var command = new CancelLeaveRequestCommand { Id = 1 };
            await handler.Handle(command, CancellationToken.None);

            // Assert
            var leaveRequest = await _mockLeaveRequestRepo.Object.GetByIdAsync(1);
            leaveRequest.ShouldNotBeNull();
            leaveRequest.Cancelled.ShouldBeTrue();

            // Verify allocation update was called
            _mockLeaveAllocationRepo.Verify(x =>
                x.UpdateAsync(It.IsAny<Domain.LeaveAllocation>()), Times.Once);

            // Verify email was sent
            _mockEmailSender.Verify(e =>
                e.SendEmail(It.IsAny<EmailMessage>()), Times.Once);
        }

        [Fact]
        public async Task CancelNotApprovedLeaveRequest_ShouldNotUpdateAllocation()
        {
            // Arrange
            // Ensure request is not approved
            var notApprovedRequest = await _mockLeaveRequestRepo.Object.GetByIdAsync(1);
            notApprovedRequest.Approved = false;
            await _mockLeaveRequestRepo.Object.UpdateAsync(notApprovedRequest);

            var handler = new CancelLeaveRequestCommandHandler(
                _mockLeaveRequestRepo.Object,
                _mockLeaveAllocationRepo.Object,
                _mockEmailSender.Object);

            // Act
            var command = new CancelLeaveRequestCommand { Id = 1 };
            await handler.Handle(command, CancellationToken.None);

            // Assert
            var leaveRequest = await _mockLeaveRequestRepo.Object.GetByIdAsync(1);
            leaveRequest.ShouldNotBeNull();
            leaveRequest.Cancelled.ShouldBeTrue();

            // Verify allocation update was NOT called
            _mockLeaveAllocationRepo.Verify(x =>
                x.UpdateAsync(It.IsAny<Domain.LeaveAllocation>()), Times.Never);

            // Verify email was still sent
            _mockEmailSender.Verify(e =>
                e.SendEmail(It.IsAny<EmailMessage>()), Times.Once);
        }

        [Fact]
        public async Task CancelLeaveRequest_NotFoundTest()
        {
            // Arrange
            var handler = new CancelLeaveRequestCommandHandler(
                _mockLeaveRequestRepo.Object,
                _mockLeaveAllocationRepo.Object,
                _mockEmailSender.Object);

            // Act & Assert
            var command = new CancelLeaveRequestCommand { Id = 99 };
            await Should.ThrowAsync<NotFoundException>(async () =>
                await handler.Handle(command, CancellationToken.None));

            // Verify no allocation changes were attempted
            _mockLeaveAllocationRepo.Verify(x =>
                x.UpdateAsync(It.IsAny<Domain.LeaveAllocation>()), Times.Never);

            // Verify no email was sent
            _mockEmailSender.Verify(e =>
                e.SendEmail(It.IsAny<EmailMessage>()), Times.Never);
        }
    }
}