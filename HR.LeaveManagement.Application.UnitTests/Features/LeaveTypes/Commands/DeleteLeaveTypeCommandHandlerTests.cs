using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Logging;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveType.Commands.DeleteLeaveType;
using HR.LeaveManagement.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using Moq;
using Shouldly;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveTypes.Commands;

public class DeleteLeaveTypeCommandHandlerTests
{
    private readonly Mock<ILeaveTypeRepository> _mockRepo;
    private IMapper _mapper;
    private Mock<IAppLogger<DeleteLeaveTypeCommandHandler>> _mockAppLogger;

    public DeleteLeaveTypeCommandHandlerTests()
    {
        _mockRepo = MockLeaveTypeRepository.GetMockLeaveTypeRepository();

        var mapperConfig = new MapperConfiguration(c => { c.AddProfile<LeaveTypeProfile>(); });

        _mapper = mapperConfig.CreateMapper();
        _mockAppLogger = new Mock<IAppLogger<DeleteLeaveTypeCommandHandler>>();
    }

    [Fact]
    public async Task DeleteLeaveTypeTest()
    {
        var handler = new DeleteLeaveTypeCommandHandler(_mockRepo.Object);

        var command = new DeleteLeaveTypeCommand { Id = 1 };
        await handler.Handle(command, CancellationToken.None);

        var leaveType = await _mockRepo.Object.GetByIdAsync(1);
        leaveType.ShouldBeNull();
    }

    [Fact]
    public async Task DeleteLeaveType_NotFoundTest()
    {
        var handler = new DeleteLeaveTypeCommandHandler(_mockRepo.Object);

        var command = new DeleteLeaveTypeCommand { Id = 99 };
        await Should.ThrowAsync<NotFoundException>(async () => await handler.Handle(command, CancellationToken.None));
    }
}