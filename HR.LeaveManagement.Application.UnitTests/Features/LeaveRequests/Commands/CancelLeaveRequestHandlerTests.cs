using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Email;
using HR.LeaveManagement.Application.Contracts.Logging;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveRequest.Commands.CancelLeaveRequest;
using HR.LeaveManagement.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using Moq;
using Shouldly;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveRequests.Commands;

public class CancelLeaveRequestHandlerTests
{
    private readonly Mock<ILeaveRequestRepository> _mockLeaveRequestRepo;
    private IMapper _mapper;
    private Mock<IAppLogger<CancelLeaveRequestCommandHandler>> _mockAppLogger;
    private readonly Mock<IEmailSender> _mockEmailSender;

    public CancelLeaveRequestHandlerTests()
    {
        _mockLeaveRequestRepo = MockLeaveRequestRepository.GetMockLeaveRequestRepository();

        var mapperConfig = new MapperConfiguration(c => { c.AddProfile<LeaveRequestProfile>(); });

        _mapper = mapperConfig.CreateMapper();
        _mockAppLogger = new Mock<IAppLogger<CancelLeaveRequestCommandHandler>>();
        _mockEmailSender = new Mock<IEmailSender>();
    }

    [Fact]
    public async Task CancelLeaveRequestTest()
    {
        var handler = new CancelLeaveRequestCommandHandler(
            _mockLeaveRequestRepo.Object,
            _mockEmailSender.Object);

        var command = new CancelLeaveRequestCommand { Id = 1 };
        await handler.Handle(command, CancellationToken.None);

        var leaveRequest = await _mockLeaveRequestRepo.Object.GetByIdAsync(1);
        leaveRequest.ShouldNotBeNull();
        leaveRequest.Cancelled.ShouldBeTrue();
    }

    [Fact]
    public async Task CancelLeaveRequest_NotFoundTest()
    {
        var handler = new CancelLeaveRequestCommandHandler(
            _mockLeaveRequestRepo.Object,
            _mockEmailSender.Object);

        var command = new CancelLeaveRequestCommand { Id = 99 };
        await Should.ThrowAsync<NotFoundException>(async () => await handler.Handle(command, CancellationToken.None));
    }
}
