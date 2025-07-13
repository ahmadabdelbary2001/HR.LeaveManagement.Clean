using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Email;
using HR.LeaveManagement.Application.Contracts.Identity;
using HR.LeaveManagement.Application.Contracts.Logging;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveRequest.Commands.CreateLeaveRequest;
using HR.LeaveManagement.Application.MappingProfiles;
using HR.LeaveManagement.Application.Models.Email;
using HR.LeaveManagement.Application.Models.Identity;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using HR.LeaveManagement.Domain;
using MediatR;
using Moq;
using Shouldly;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveRequests.Commands;

public class CreateLeaveRequestHandlerTests
{
    private readonly Mock<ILeaveRequestRepository> _mockLeaveRequestRepo;
    private readonly Mock<ILeaveTypeRepository> _mockLeaveTypeRepo;
    private readonly Mock<ILeaveAllocationRepository> _mockLeaveAllocationRepo;
    private readonly Mock<IEmailSender> _mockEmailSender;
    private readonly Mock<IUserService> _mockUserService;
    private readonly IMapper _mapper;
    private readonly Mock<IAppLogger<CreateLeaveRequestCommandHandler>> _mockAppLogger;

    public CreateLeaveRequestHandlerTests()
    {
        _mockLeaveRequestRepo = MockLeaveRequestRepository.GetMockLeaveRequestRepository();
        _mockLeaveTypeRepo = MockLeaveTypeRepository.GetMockLeaveTypeRepository();
        _mockLeaveAllocationRepo = new Mock<ILeaveAllocationRepository>();
        _mockEmailSender = new Mock<IEmailSender>();
        _mockUserService = new Mock<IUserService>();
        _mockAppLogger = new Mock<IAppLogger<CreateLeaveRequestCommandHandler>>();

        var mapperConfig = new MapperConfiguration(c => { 
            c.AddProfile<LeaveRequestProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

        // Setup user service defaults
        _mockUserService.Setup(x => x.UserId).Returns("user1");
        _mockUserService.Setup(x => x.GetEmployee(It.IsAny<string>()))
            .ReturnsAsync(new Employee { Email = "test@example.com" });
    }

    private CreateLeaveRequestCommandHandler CreateHandler()
    {
        return new CreateLeaveRequestCommandHandler(
            _mockEmailSender.Object,
            _mapper,
            _mockLeaveTypeRepo.Object,
            _mockLeaveRequestRepo.Object,
            _mockLeaveAllocationRepo.Object,
            _mockUserService.Object);
    }

    [Fact]
    public async Task CreateLeaveRequestTest()
    {
        // Arrange
        _mockLeaveAllocationRepo.Setup(x => x.GetUserAllocations(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(new LeaveAllocation { NumberOfDays = 10 });
        
        var handler = CreateHandler();
        var command = new CreateLeaveRequestCommand
        {
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(5),
            LeaveTypeId = 1,
            RequestComments = "Test request",
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Unit>();
        _mockLeaveRequestRepo.Verify(r => r.CreateAsync(It.IsAny<LeaveRequest>()), Times.Once);
        _mockEmailSender.Verify(e => e.SendEmail(It.IsAny<EmailMessage>()), Times.Once);
    }

    [Fact]
    public async Task CreateLeaveRequest_ValidationFailTest()
    {
        // Arrange
        var handler = CreateHandler();
        var command = new CreateLeaveRequestCommand
        {
            StartDate = DateTime.Now.AddDays(5),
            EndDate = DateTime.Now.AddDays(1), // Invalid end date
            LeaveTypeId = 1,
            RequestComments = "Test request",
        };

        // Act & Assert
        await Should.ThrowAsync<BadRequestException>(async () => 
            await handler.Handle(command, CancellationToken.None));

        _mockLeaveRequestRepo.Verify(r => r.CreateAsync(It.IsAny<LeaveRequest>()), Times.Never);
        _mockEmailSender.Verify(e => e.SendEmail(It.IsAny<EmailMessage>()), Times.Never);
    }

    [Fact]
    public async Task CreateLeaveRequest_AllocationNotFoundTest()
    {
        // Arrange
        _mockLeaveAllocationRepo.Setup(x => x.GetUserAllocations(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync((LeaveAllocation)null); // Simulate no allocation
        
        var handler = CreateHandler();
        var command = new CreateLeaveRequestCommand
        {
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(2),
            LeaveTypeId = 1,
            RequestComments = "Test request",
        };

        // Act & Assert
        await Should.ThrowAsync<BadRequestException>(async () => 
            await handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task CreateLeaveRequest_InsufficientDaysTest()
    {
        // Arrange
        _mockLeaveAllocationRepo.Setup(x => x.GetUserAllocations(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(new LeaveAllocation { NumberOfDays = 1 }); // Only 1 day available
        
        var handler = CreateHandler();
        var command = new CreateLeaveRequestCommand
        {
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(3), // 2 business days requested
            LeaveTypeId = 1,
            RequestComments = "Test request",
        };

        // Act & Assert
        await Should.ThrowAsync<BadRequestException>(async () => 
            await handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task CreateLeaveRequest_EmailFailureTest()
    {
        // Arrange
        _mockLeaveAllocationRepo.Setup(x => x.GetUserAllocations(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(new LeaveAllocation { NumberOfDays = 10 });
        
        _mockEmailSender.Setup(x => x.SendEmail(It.IsAny<EmailMessage>()))
            .ThrowsAsync(new Exception("Email failed"));
        
        var handler = CreateHandler();
        var command = new CreateLeaveRequestCommand
        {
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(2),
            LeaveTypeId = 1,
            RequestComments = "Test request",
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert - Should succeed even if email fails
        result.ShouldBeOfType<Unit>();
        _mockLeaveRequestRepo.Verify(r => r.CreateAsync(It.IsAny<LeaveRequest>()), Times.Once);
    }
}