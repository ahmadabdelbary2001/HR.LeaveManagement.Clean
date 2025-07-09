using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Logging;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveAllocation.Commands.DeleteLeaveAllocation;
using HR.LeaveManagement.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using Moq;
using Shouldly;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveAllocations.Commands;

public class DeleteLeaveAllocationHandlerTests
{
    private readonly Mock<ILeaveAllocationRepository> _mockRepo;
    private IMapper _mapper;
    private Mock<IAppLogger<DeleteLeaveAllocationCommandHandler>> _mockAppLogger;

    public DeleteLeaveAllocationHandlerTests()
    {
        _mockRepo = MockLeaveAllocationRepository.GetMockLeaveAllocationRepository();

        var mapperConfig = new MapperConfiguration(c => { c.AddProfile<LeaveAllocationProfile>(); });

        _mapper = mapperConfig.CreateMapper();
        _mockAppLogger = new Mock<IAppLogger<DeleteLeaveAllocationCommandHandler>>();
    }

    [Fact]
    public async Task DeleteLeaveAllocationTest()
    {
        var handler = new DeleteLeaveAllocationCommandHandler(_mockRepo.Object, _mapper);

        var command = new DeleteLeaveAllocationCommand { Id = 1 };
        await handler.Handle(command, CancellationToken.None);

        var leaveAllocation = await _mockRepo.Object.GetByIdAsync(1);
        leaveAllocation.ShouldBeNull();
    }

    [Fact]
    public async Task DeleteLeaveAllocation_NotFoundTest()
    {
        var handler = new DeleteLeaveAllocationCommandHandler(_mockRepo.Object, _mapper);

        var command = new DeleteLeaveAllocationCommand { Id = 99 };
        await Should.ThrowAsync<NotFoundException>(async () => await handler.Handle(command, CancellationToken.None));
    }
}