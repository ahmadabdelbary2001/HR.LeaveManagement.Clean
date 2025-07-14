using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Email; // Ensure correct namespace
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveRequest.Commands.ChangeLeaveRequestApproval;
using HR.LeaveManagement.Application.MappingProfiles;
using HR.LeaveManagement.Application.Models.Email;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using MediatR;
using Moq;
using Shouldly;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveRequests.Commands
{
    public class ChangeLeaveRequestApprovalHandlerTests
    {
        private readonly Mock<ILeaveRequestRepository> _mockLeaveRequestRepo;
        private readonly Mock<ILeaveTypeRepository> _mockLeaveTypeRepo;
        private readonly Mock<ILeaveAllocationRepository> _mockLeaveAllocationRepo;
        private readonly Mock<IEmailSender> _mockEmailSender;
        private readonly IMapper _mapper;

        public ChangeLeaveRequestApprovalHandlerTests()
        {
            _mockLeaveRequestRepo = MockLeaveRequestRepository.GetMockLeaveRequestRepository();
            _mockLeaveTypeRepo = MockLeaveTypeRepository.GetMockLeaveTypeRepository();
            _mockLeaveAllocationRepo = new Mock<ILeaveAllocationRepository>();
            _mockEmailSender = new Mock<IEmailSender>();

            var mapperConfig = new MapperConfiguration(c => { 
                c.AddProfile<LeaveRequestProfile>(); 
            });

            _mapper = mapperConfig.CreateMapper();
            
            // Setup allocation mock
            _mockLeaveAllocationRepo.Setup(x => 
                    x.GetUserAllocations(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(new Domain.LeaveAllocation
                {
                    Id = 1,
                    NumberOfDays = 10
                });
        }

        [Fact]
        public async Task ChangeLeaveRequestApprovalTest()
        {
            var handler = new ChangeLeaveRequestApprovalCommandHandler(
                _mockLeaveRequestRepo.Object,
                _mockLeaveTypeRepo.Object,
                _mockLeaveAllocationRepo.Object,
                _mapper,
                _mockEmailSender.Object);

            var command = new ChangeLeaveRequestApprovalCommand { Id = 1, Approved = true };
            var result = await handler.Handle(command, CancellationToken.None);

            result.ShouldBeOfType<Unit>();
            var leaveRequest = await _mockLeaveRequestRepo.Object.GetByIdAsync(1);
            leaveRequest.ShouldNotBeNull();
            leaveRequest.Approved.ShouldBe(true);
            
            _mockLeaveAllocationRepo.Verify(x => 
                x.UpdateAsync(It.IsAny<Domain.LeaveAllocation>()), Times.Once);
            
            // Fixed method name and namespace
            _mockEmailSender.Verify(e => 
                e.SendEmail(It.IsAny<EmailMessage>()), Times.Once);
        }

        [Fact]
        public async Task ChangeLeaveRequestApproval_NotFoundTest()
        {
            var handler = new ChangeLeaveRequestApprovalCommandHandler(
                _mockLeaveRequestRepo.Object,
                _mockLeaveTypeRepo.Object,
                _mockLeaveAllocationRepo.Object,
                _mapper,
                _mockEmailSender.Object);

            var command = new ChangeLeaveRequestApprovalCommand { Id = 99, Approved = true };
            await Should.ThrowAsync<NotFoundException>(async () => 
                await handler.Handle(command, CancellationToken.None));

            _mockLeaveAllocationRepo.Verify(x => 
                x.UpdateAsync(It.IsAny<Domain.LeaveAllocation>()), Times.Never);
            
            // Fixed method name and namespace
            _mockEmailSender.Verify(e => 
                e.SendEmail(It.IsAny<EmailMessage>()), Times.Never);
        }
    }
}