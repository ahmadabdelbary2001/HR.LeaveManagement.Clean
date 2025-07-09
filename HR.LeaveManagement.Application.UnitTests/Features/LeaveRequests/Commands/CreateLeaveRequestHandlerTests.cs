using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Email;
using HR.LeaveManagement.Application.Contracts.Logging;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveRequest.Commands.CreateLeaveRequest;
using HR.LeaveManagement.Application.MappingProfiles;
using HR.LeaveManagement.Application.Models.Email;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using MediatR;
using Moq;
using Shouldly;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveRequests.Commands;

public class CreateLeaveRequestHandlerTests
{
    private readonly Mock<ILeaveRequestRepository> _mockLeaveRequestRepo;
    private readonly Mock<ILeaveTypeRepository> _mockLeaveTypeRepo;
    private readonly Mock<IEmailSender> _mockEmailSender;
    private IMapper _mapper;
    private Mock<IAppLogger<CreateLeaveRequestCommandHandler>> _mockAppLogger;

    public CreateLeaveRequestHandlerTests()
    {
        _mockLeaveRequestRepo = MockLeaveRequestRepository.GetMockLeaveRequestRepository();
        _mockLeaveTypeRepo = MockLeaveTypeRepository.GetMockLeaveTypeRepository();
        _mockEmailSender = new Mock<IEmailSender>();

        var mapperConfig = new MapperConfiguration(c => { c.AddProfile<LeaveRequestProfile>(); });

        _mapper = mapperConfig.CreateMapper();
        _mockAppLogger = new Mock<IAppLogger<CreateLeaveRequestCommandHandler>>();
    }

    [Fact]
    public async Task CreateLeaveRequestTest()
    {
        var handler = new CreateLeaveRequestCommandHandler(
            _mockEmailSender.Object,
            _mapper,
            _mockLeaveTypeRepo.Object,
            _mockLeaveRequestRepo.Object);

        var command = new CreateLeaveRequestCommand
        {
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(5),
            LeaveTypeId = 1,
            RequestComments = "Test request",
        };

        var result = await handler.Handle(command, CancellationToken.None);

        result.ShouldBeOfType<Unit>();
        _mockLeaveRequestRepo.Verify(r => r.CreateAsync(It.IsAny<HR.LeaveManagement.Domain.LeaveRequest>()), Times.Once);
        _mockEmailSender.Verify(e => e.SendEmail(It.IsAny<EmailMessage>()), Times.Once);
    }

    [Fact]
    public async Task CreateLeaveRequest_ValidationFailTest()
    {
        var handler = new CreateLeaveRequestCommandHandler(
            _mockEmailSender.Object,
            _mapper,
            _mockLeaveTypeRepo.Object,
            _mockLeaveRequestRepo.Object);

        var command = new CreateLeaveRequestCommand
        {
            StartDate = DateTime.Now.AddDays(5),
            EndDate = DateTime.Now.AddDays(1),
            LeaveTypeId = 1,
            RequestComments = "Test request",
        };

        await Should.ThrowAsync<BadRequestException>(async () => await handler.Handle(command, CancellationToken.None));

        _mockLeaveRequestRepo.Verify(r => r.CreateAsync(It.IsAny<HR.LeaveManagement.Domain.LeaveRequest>()), Times.Never);
        _mockEmailSender.Verify(e => e.SendEmail(It.IsAny<EmailMessage>()), Times.Never);
    }
}