using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Logging;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveAllocation.Commands.UpdateLeaveAllocation;
using HR.LeaveManagement.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using MediatR;
using Moq;
using Shouldly;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveAllocations.Commands;

public class UpdateLeaveAllocationCommandHandlerTests
{
    private readonly Mock<ILeaveAllocationRepository> _mockRepo;
    private readonly Mock<ILeaveTypeRepository> _mockLeaveTypeRepo;
    private IMapper _mapper;
    private Mock<IAppLogger<UpdateLeaveAllocationCommandHandler>> _mockAppLogger;

    public UpdateLeaveAllocationCommandHandlerTests()
    {
        _mockRepo = MockLeaveAllocationRepository.GetMockLeaveAllocationRepository();
        _mockLeaveTypeRepo = MockLeaveTypeRepository.GetMockLeaveTypeRepository();

        var mapperConfig = new MapperConfiguration(c => { c.AddProfile<LeaveAllocationProfile>(); });

        _mapper = mapperConfig.CreateMapper();
        _mockAppLogger = new Mock<IAppLogger<UpdateLeaveAllocationCommandHandler>>();
    }

    [Fact]
    public async Task UpdateLeaveAllocationTest()
    {
        var handler = new UpdateLeaveAllocationCommandHandler(
            _mapper,
            _mockLeaveTypeRepo.Object,
            _mockRepo.Object);

        var command = new UpdateLeaveAllocationCommand { Id = 1, NumberOfDays = 12, LeaveTypeId = 1, Period = 2025 };
        var result = await handler.Handle(command, CancellationToken.None);

        result.ShouldBeOfType<Unit>();
        var leaveAllocation = await _mockRepo.Object.GetByIdAsync(1);
        leaveAllocation.ShouldNotBeNull();
        leaveAllocation.NumberOfDays.ShouldBe(12);
    }

    [Fact]
    public async Task UpdateLeaveAllocation_ValidationFailTest()
    {
        var handler = new UpdateLeaveAllocationCommandHandler(
            _mapper,
            _mockLeaveTypeRepo.Object,
            _mockRepo.Object);

        var command = new UpdateLeaveAllocationCommand { Id = 1, NumberOfDays = -5, LeaveTypeId = 1, Period = 2025 };
        await Should.ThrowAsync<BadRequestException>(async () => await handler.Handle(command, CancellationToken.None));

        var leaveAllocation = await _mockRepo.Object.GetByIdAsync(1);
        leaveAllocation.ShouldNotBeNull();
        leaveAllocation.NumberOfDays.ShouldNotBe(-5);
    }
}