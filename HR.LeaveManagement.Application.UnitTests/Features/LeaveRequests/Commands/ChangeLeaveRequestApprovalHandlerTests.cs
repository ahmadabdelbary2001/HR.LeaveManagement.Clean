using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Email;
using HR.LeaveManagement.Application.Contracts.Logging;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveRequest.Commands.ChangeLeaveRequestApproval;
using HR.LeaveManagement.Application.MappingProfiles;
using HR.LeaveManagement.Application.Models.Email;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using MediatR;
using Moq;
using Shouldly;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveRequests.Commands;

public class ChangeLeaveRequestApprovalHandlerTests
{
    private readonly Mock<ILeaveRequestRepository> _mockLeaveRequestRepo;
    private readonly Mock<ILeaveTypeRepository> _mockLeaveTypeRepo;
    private readonly Mock<IEmailSender> _mockEmailSender;
    private IMapper _mapper;
    private Mock<IAppLogger<ChangeLeaveRequestApprovalCommandHandler>> _mockAppLogger;

    public ChangeLeaveRequestApprovalHandlerTests()
    {
        _mockLeaveRequestRepo = MockLeaveRequestRepository.GetMockLeaveRequestRepository();
        _mockLeaveTypeRepo = MockLeaveTypeRepository.GetMockLeaveTypeRepository();
        _mockEmailSender = new Mock<IEmailSender>();

        var mapperConfig = new MapperConfiguration(c => { c.AddProfile<LeaveRequestProfile>(); });

        _mapper = mapperConfig.CreateMapper();
        _mockAppLogger = new Mock<IAppLogger<ChangeLeaveRequestApprovalCommandHandler>>();
    }

    [Fact]
    public async Task ChangeLeaveRequestApprovalTest()
    {
        var handler = new ChangeLeaveRequestApprovalCommandHandler(
            _mockLeaveRequestRepo.Object,
            _mockLeaveTypeRepo.Object,
            _mapper,
            _mockEmailSender.Object);

        var command = new ChangeLeaveRequestApprovalCommand { Id = 1, Approved = true };
        var result = await handler.Handle(command, CancellationToken.None);

        result.ShouldBeOfType<Unit>();
        var leaveRequest = await _mockLeaveRequestRepo.Object.GetByIdAsync(1);
        leaveRequest.ShouldNotBeNull();
        ((bool)leaveRequest.Approved).ShouldBeTrue();
        _mockEmailSender.Verify(e => e.SendEmail(It.IsAny<EmailMessage>()), Times.Once);
    }

    [Fact]
    public async Task ChangeLeaveRequestApproval_NotFoundTest()
    {
        var handler = new ChangeLeaveRequestApprovalCommandHandler(
            _mockLeaveRequestRepo.Object,
            _mockLeaveTypeRepo.Object,
            _mapper,
            _mockEmailSender.Object);

        var command = new ChangeLeaveRequestApprovalCommand { Id = 99, Approved = true };
        await Should.ThrowAsync<NotFoundException>(async () => await handler.Handle(command, CancellationToken.None));

        _mockEmailSender.Verify(e => e.SendEmail(It.IsAny<EmailMessage>()), Times.Never);
    }
}