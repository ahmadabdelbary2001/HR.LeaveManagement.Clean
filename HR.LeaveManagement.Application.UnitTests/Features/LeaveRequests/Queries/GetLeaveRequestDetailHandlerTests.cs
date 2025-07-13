using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Identity;
using HR.LeaveManagement.Application.Contracts.Logging;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveRequest.Queries.GetLeaveRequestDetail;
using HR.LeaveManagement.Application.MappingProfiles;
using HR.LeaveManagement.Application.Models.Identity;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using Moq;
using Shouldly;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveRequests.Queries;

public class GetLeaveRequestDetailHandlerTests
{
    private readonly Mock<ILeaveRequestRepository> _mockRepo;
    private readonly Mock<IUserService> _mockUserService; // Added mock
    private readonly IMapper _mapper;

    public GetLeaveRequestDetailHandlerTests()
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
    public async Task GetLeaveRequestDetailsTest()
    {
        // Add mock user service to handler instantiation
        var handler = new GetLeaveRequestDetailQueryHandler(
            _mockRepo.Object,
            _mapper,
            _mockUserService.Object); // Added dependency

        var result = await handler.Handle(
            new GetLeaveRequestDetailQuery { Id = 1 },
            CancellationToken.None);

        result.ShouldBeOfType<LeaveRequestDetailsDto>();
        result.Id.ShouldBe(1);
    }

    [Fact]
    public async Task GetLeaveRequestDetails_NotFoundTest()
    {
        // Add mock user service to handler instantiation
        var handler = new GetLeaveRequestDetailQueryHandler(
            _mockRepo.Object,
            _mapper,
            _mockUserService.Object); // Added dependency

        var command = new GetLeaveRequestDetailQuery { Id = 99 };
        await Should.ThrowAsync<NotFoundException>(async () =>
            await handler.Handle(command, CancellationToken.None));
    }
}