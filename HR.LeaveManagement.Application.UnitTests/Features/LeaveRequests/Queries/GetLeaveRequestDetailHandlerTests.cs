using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Logging;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveRequest.Queries.GetLeaveRequestDetail;
using HR.LeaveManagement.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using Moq;
using Shouldly;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveRequests.Queries;

public class GetLeaveRequestDetailHandlerTests
{
    private readonly Mock<ILeaveRequestRepository> _mockRepo;
    private IMapper _mapper;
    private Mock<IAppLogger<GetLeaveRequestDetailQueryHandler>> _mockAppLogger;

    public GetLeaveRequestDetailHandlerTests()
    {
        _mockRepo = MockLeaveRequestRepository.GetMockLeaveRequestRepository();

        var mapperConfig = new MapperConfiguration(c => { c.AddProfile<LeaveRequestProfile>(); });

        _mapper = mapperConfig.CreateMapper();
        _mockAppLogger = new Mock<IAppLogger<GetLeaveRequestDetailQueryHandler>>();
    }

    [Fact]
    public async Task GetLeaveRequestDetailsTest()
    {
        var handler = new GetLeaveRequestDetailQueryHandler(_mockRepo.Object, _mapper);

        var result = await handler.Handle(new GetLeaveRequestDetailQuery { Id = 1 }, CancellationToken.None);

        result.ShouldBeOfType<LeaveRequestDetailsDto>();
        result.Id.ShouldBe(1);
    }

    [Fact]
    public async Task GetLeaveRequestDetails_NotFoundTest()
    {
        var handler = new GetLeaveRequestDetailQueryHandler(_mockRepo.Object, _mapper);

        var command = new GetLeaveRequestDetailQuery { Id = 99 };
        await Should.ThrowAsync<NotFoundException>(async () => await handler.Handle(command, CancellationToken.None));
    }
}