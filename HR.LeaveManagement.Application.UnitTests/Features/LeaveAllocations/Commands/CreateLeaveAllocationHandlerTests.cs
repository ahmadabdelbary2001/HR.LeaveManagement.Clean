using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Identity;
using HR.LeaveManagement.Application.Contracts.Logging;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveAllocation.Commands.CreateLeaveAllocation;
using HR.LeaveManagement.Application.MappingProfiles;
using HR.LeaveManagement.Application.Models.Identity;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using MediatR;
using Moq;
using Shouldly;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveAllocations.Commands;

public class CreateLeaveAllocationHandlerTests
{
    private readonly Mock<ILeaveAllocationRepository> _mockRepo;
    private readonly Mock<ILeaveTypeRepository> _mockLeaveTypeRepo;
    private readonly Mock<IUserService> _userService;
    private readonly IMapper _mapper;

    public CreateLeaveAllocationHandlerTests()
    {
        _mockRepo = MockLeaveAllocationRepository.GetMockLeaveAllocationRepository();
        _mockLeaveTypeRepo = MockLeaveTypeRepository.GetMockLeaveTypeRepository();
        _userService = new Mock<IUserService>();

        // Setup employee data
        _userService.Setup(s => s.GetEmployees())
            .ReturnsAsync(new List<Employee>
            {
                new Employee { Id = "emp1", Email = "emp1@test.com" },
                new Employee { Id = "emp2", Email = "emp2@test.com" }
            });

        var mapperConfig = new MapperConfiguration(c => 
        {
            c.AddProfile<LeaveAllocationProfile>();
        });
        _mapper = mapperConfig.CreateMapper();
    }

    [Fact]
    public async Task CreateLeaveAllocationTest()
    {
        // Arrange
        var handler = new CreateLeaveAllocationCommandHandler(
            _mapper, 
            _mockRepo.Object, 
            _mockLeaveTypeRepo.Object, 
            _userService.Object);

        // Act
        var result = await handler.Handle(
            new CreateLeaveAllocationCommand { LeaveTypeId = 1 }, 
            CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Unit>();
        
        // Verify allocations were created
        _mockRepo.Verify(r => r.AddAllocations(It.IsAny<List<Domain.LeaveAllocation>>()), Times.Once);
    }

    [Fact]
    public async Task CreateLeaveAllocation_ValidationFailTest()
    {
        // Arrange
        var handler = new CreateLeaveAllocationCommandHandler(
            _mapper, 
            _mockRepo.Object, 
            _mockLeaveTypeRepo.Object, 
            _userService.Object);

        var command = new CreateLeaveAllocationCommand { LeaveTypeId = 99 };

        // Act & Assert
        await Should.ThrowAsync<BadRequestException>(
            async () => await handler.Handle(command, CancellationToken.None));
    }
}