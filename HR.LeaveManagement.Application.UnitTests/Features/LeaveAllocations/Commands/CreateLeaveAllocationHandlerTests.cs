using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Logging;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveAllocation.Commands.CreateLeaveAllocation;
using HR.LeaveManagement.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using MediatR;
using Moq;
using Shouldly;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveAllocations.Commands;

public class CreateLeaveAllocationHandlerTests
{
    private readonly Mock<ILeaveAllocationRepository> _mockRepo;
    private readonly Mock<ILeaveTypeRepository> _mockLeaveTypeRepo;
    private IMapper _mapper;
    private Mock<IAppLogger<CreateLeaveAllocationCommandHandler>> _mockAppLogger;

    public CreateLeaveAllocationHandlerTests()
    {
        _mockRepo = MockLeaveAllocationRepository.GetMockLeaveAllocationRepository();
        _mockLeaveTypeRepo = MockLeaveTypeRepository.GetMockLeaveTypeRepository();

        var mapperConfig = new MapperConfiguration(c => { c.AddProfile<LeaveAllocationProfile>(); });

        _mapper = mapperConfig.CreateMapper();
        _mockAppLogger = new Mock<IAppLogger<CreateLeaveAllocationCommandHandler>>();
    }

    [Fact]
    public async Task CreateLeaveAllocationTest()
    {
        var handler = new CreateLeaveAllocationCommandHandler(_mapper, _mockRepo.Object, _mockLeaveTypeRepo.Object);

        var result = await handler.Handle(new CreateLeaveAllocationCommand { LeaveTypeId = 1 }, CancellationToken.None);

        result.ShouldBeOfType<Unit>();
    }

    [Fact]
    public async Task CreateLeaveAllocation_ValidationFailTest()
    {
        var handler = new CreateLeaveAllocationCommandHandler(_mapper, _mockRepo.Object, _mockLeaveTypeRepo.Object);

        var command = new CreateLeaveAllocationCommand { LeaveTypeId = 99 };
        await Should.ThrowAsync<BadRequestException>(async () => await handler.Handle(command, CancellationToken.None));
    }
}