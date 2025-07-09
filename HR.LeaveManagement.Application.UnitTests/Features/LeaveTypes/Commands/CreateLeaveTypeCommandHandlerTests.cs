using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using HR.LeaveManagement.Application.Contracts.Logging;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveType.Commands.CreateLeaveType;
using HR.LeaveManagement.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using Moq;
using Shouldly;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveTypes.Commands;

public class CreateLeaveTypeCommandHandlerTests
{
    private readonly Mock<ILeaveTypeRepository> _mockRepo;
    private IMapper _mapper;
    private Mock<IAppLogger<CreateLeaveTypeCommandHandler>> _mockAppLogger;
    private Mock<IValidator<CreateLeaveTypeCommand>> _mockValidator;

    public CreateLeaveTypeCommandHandlerTests()
    {
        _mockRepo = MockLeaveTypeRepository.GetMockLeaveTypeRepository();

        var mapperConfig = new MapperConfiguration(c => { c.AddProfile<LeaveTypeProfile>(); });

        _mapper = mapperConfig.CreateMapper();
        _mockAppLogger = new Mock<IAppLogger<CreateLeaveTypeCommandHandler>>();
        _mockValidator = new Mock<IValidator<CreateLeaveTypeCommand>>();
    }

    [Fact]
    public async Task CreateLeaveTypeTest()
    {
        _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<CreateLeaveTypeCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure>()));

        var handler = new CreateLeaveTypeCommandHandler(_mapper, _mockRepo.Object);

        var result = await handler.Handle(new CreateLeaveTypeCommand { Name = "Test", DefaultDays = 10 }, CancellationToken.None);

        result.ShouldBeOfType<int>();
        result.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task CreateLeaveType_ValidationFailTest()
    {
        _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<CreateLeaveTypeCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Name", "Name is required") }));

        var handler = new CreateLeaveTypeCommandHandler(_mapper, _mockRepo.Object);

        var command = new CreateLeaveTypeCommand { Name = "", DefaultDays = 10 };
        await Should.ThrowAsync<BadRequestException>(async () => await handler.Handle(command, CancellationToken.None));
    }
}