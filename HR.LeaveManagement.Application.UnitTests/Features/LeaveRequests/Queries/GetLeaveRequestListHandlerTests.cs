using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Identity;
using HR.LeaveManagement.Application.Contracts.Logging;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Features.LeaveRequest.Queries.GetLeaveRequestList;
using HR.LeaveManagement.Application.MappingProfiles;
using HR.LeaveManagement.Application.Models.Identity;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using Moq;
using Shouldly;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveRequests.Queries;

public class GetLeaveRequestListHandlerTests
{
    private readonly Mock<ILeaveRequestRepository> _mockRepo;
    private readonly Mock<IUserService> _mockUserService; // Added mock
    private readonly IMapper _mapper;

    public GetLeaveRequestListHandlerTests()
    {
        _mockRepo = MockLeaveRequestRepository.GetMockLeaveRequestRepository();
        _mockUserService = new Mock<IUserService>(); // Initialize

        var mapperConfig = new MapperConfiguration(c => { c.AddProfile<LeaveRequestProfile>(); });

        _mapper = mapperConfig.CreateMapper();

        // Setup mock response for employee data
        _mockUserService.Setup(s => s.GetEmployee(It.IsAny<string>()))
            .ReturnsAsync(new Employee()); // Dummy employee
    }

    [Fact]
    public async Task GetLeaveRequestListTest()
    {
        // Add mock user service to handler instantiation
        var handler = new GetLeaveRequestListQueryHandler(
            _mockRepo.Object,
            _mapper,
            _mockUserService.Object); // Added dependency

        var result = await handler.Handle(new GetLeaveRequestListQuery(), CancellationToken.None);

        result.ShouldBeOfType<List<LeaveRequestListDto>>();
        result.Count.ShouldBe(2);
    }
}