using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Logging;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Features.LeaveRequest.Queries.GetLeaveRequestList;
using HR.LeaveManagement.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using Moq;
using Shouldly;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveRequests.Queries;

public class GetLeaveRequestListHandlerTests
{
    private readonly Mock<ILeaveRequestRepository> _mockRepo;
    private IMapper _mapper;
    private Mock<IAppLogger<GetLeaveRequestListQueryHandler>> _mockAppLogger;

    public GetLeaveRequestListHandlerTests()
    {
        _mockRepo = MockLeaveRequestRepository.GetMockLeaveRequestRepository();

        var mapperConfig = new MapperConfiguration(c => { c.AddProfile<LeaveRequestProfile>(); });

        _mapper = mapperConfig.CreateMapper();
        _mockAppLogger = new Mock<IAppLogger<GetLeaveRequestListQueryHandler>>();
    }

    [Fact]
    public async Task GetLeaveRequestListTest()
    {
        var handler = new GetLeaveRequestListQueryHandler(_mockRepo.Object, _mapper);

        var result = await handler.Handle(new GetLeaveRequestListQuery(), CancellationToken.None);

        result.ShouldBeOfType<List<LeaveRequestListDto>>();
        result.Count.ShouldBe(2);
    }
}