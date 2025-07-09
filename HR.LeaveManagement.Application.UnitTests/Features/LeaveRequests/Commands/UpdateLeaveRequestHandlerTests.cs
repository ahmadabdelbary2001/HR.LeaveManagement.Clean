using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Email;
using HR.LeaveManagement.Application.Contracts.Logging;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveRequest.Commands.UpdateLeaveRequest;
using HR.LeaveManagement.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using MediatR;
using Moq;
using Shouldly;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveRequests.Commands;

public class UpdateLeaveRequestHandlerTests
{
    private readonly Mock<ILeaveRequestRepository> _mockLeaveRequestRepo;
    private readonly Mock<ILeaveTypeRepository> _mockLeaveTypeRepo;
    private readonly Mock<IEmailSender> _mockEmailSender;
    private IMapper _mapper;
    private Mock<IAppLogger<UpdateLeaveRequestCommandHandler>> _mockAppLogger;

    public UpdateLeaveRequestHandlerTests()
    {
        _mockLeaveRequestRepo = MockLeaveRequestRepository.GetMockLeaveRequestRepository();
        _mockLeaveTypeRepo = MockLeaveTypeRepository.GetMockLeaveTypeRepository();
        _mockEmailSender = new Mock<IEmailSender>();

        var mapperConfig = new MapperConfiguration(c => { c.AddProfile<LeaveRequestProfile>(); });

        _mapper = mapperConfig.CreateMapper();
        _mockAppLogger = new Mock<IAppLogger<UpdateLeaveRequestCommandHandler>>();
    }

    [Fact]
    public async Task UpdateLeaveRequestTest()
    {
        var handler = new UpdateLeaveRequestCommandHandler(
            _mockLeaveRequestRepo.Object,
            _mockLeaveTypeRepo.Object,
            _mapper,
            _mockEmailSender.Object,
            _mockAppLogger.Object);

        var command = new UpdateLeaveRequestCommand
        {
            Id = 1,
            StartDate = DateTime.Now.AddDays(2),
            EndDate = DateTime.Now.AddDays(6),
            LeaveTypeId = 1,
            RequestComments = "Updated test request",
            Cancelled = false
        };

        var result = await handler.Handle(command, CancellationToken.None);

        result.ShouldBeOfType<Unit>();
        var leaveRequest = await _mockLeaveRequestRepo.Object.GetByIdAsync(1);
        leaveRequest.ShouldNotBeNull();
        leaveRequest.RequestComments.ShouldBe("Updated test request");
    }

    [Fact]
    public async Task UpdateLeaveRequest_ValidationFailTest()
    {
        var handler = new UpdateLeaveRequestCommandHandler(
            _mockLeaveRequestRepo.Object,
            _mockLeaveTypeRepo.Object,
            _mapper,
            _mockEmailSender.Object,
            _mockAppLogger.Object);

        var command = new UpdateLeaveRequestCommand
        {
            Id = 1,
            StartDate = DateTime.Now.AddDays(5),
            EndDate = DateTime.Now.AddDays(1),
            LeaveTypeId = 1,
            RequestComments = "Updated test request",
            Cancelled = false
        };

        await Should.ThrowAsync<BadRequestException>(async () => await handler.Handle(command, CancellationToken.None));

        var leaveRequest = await _mockLeaveRequestRepo.Object.GetByIdAsync(1);
        leaveRequest.ShouldNotBeNull();
        leaveRequest.StartDate.ShouldNotBe(command.StartDate);
    }
}