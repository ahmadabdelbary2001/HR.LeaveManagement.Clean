using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using HR.LeaveManagement.Application.Contracts.Logging;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveType.Commands.UpdateLeaveType;
using HR.LeaveManagement.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using Moq;
using Shouldly;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveTypes.Commands;

public class UpdateLeaveTypeCommandHandlerTests
{
    private readonly Mock<ILeaveTypeRepository> _mockRepo;
    private IMapper _mapper;
    private Mock<IAppLogger<UpdateLeaveTypeCommandHandler>> _mockAppLogger;
    private Mock<IValidator<UpdateLeaveTypeCommand>> _mockValidator;

    public UpdateLeaveTypeCommandHandlerTests()
    {
        _mockRepo = MockLeaveTypeRepository.GetMockLeaveTypeRepository();

        var mapperConfig = new MapperConfiguration(c => { c.AddProfile<LeaveTypeProfile>(); });

        _mapper = mapperConfig.CreateMapper();
        _mockAppLogger = new Mock<IAppLogger<UpdateLeaveTypeCommandHandler>>();
        _mockValidator = new Mock<IValidator<UpdateLeaveTypeCommand>>();
    }

    [Fact]
    public async Task UpdateLeaveTypeTest()
    {
        _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<UpdateLeaveTypeCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure>()));

        var handler =
            new UpdateLeaveTypeCommandHandler(_mapper, _mockRepo.Object, _mockAppLogger.Object);

        var command = new UpdateLeaveTypeCommand { Id = 1, Name = "Updated Test", DefaultDays = 15 };
        await handler.Handle(command, CancellationToken.None);

        var leaveType = await _mockRepo.Object.GetByIdAsync(1);
        leaveType.ShouldNotBeNull();
        leaveType.Name.ShouldBe("Updated Test");
        leaveType.DefaultDays.ShouldBe(15);
    }

    [Fact]
    public async Task UpdateLeaveType_ValidationFailTest()
    {
        _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<UpdateLeaveTypeCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure>
                { new ValidationFailure("Name", "Name is required") }));

        var handler =
            new UpdateLeaveTypeCommandHandler(_mapper, _mockRepo.Object, _mockAppLogger.Object);

        var command = new UpdateLeaveTypeCommand { Id = 1, Name = "", DefaultDays = 15 };
        await Should.ThrowAsync<BadRequestException>(async () => await handler.Handle(command, CancellationToken.None));
    }
}