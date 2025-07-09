using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Logging;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveAllocation.Queries.GetLeaveAllocationDetails;
using HR.LeaveManagement.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using Moq;
using Shouldly;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveAllocations.Queries;

public class GetLeaveAllocationDetailHandlerTests
{
    private readonly Mock<ILeaveAllocationRepository> _mockRepo;
    private IMapper _mapper;
    private Mock<IAppLogger<GetLeaveAllocationDetailRequestHandler>> _mockAppLogger;

    public GetLeaveAllocationDetailHandlerTests()
    {
        _mockRepo = MockLeaveAllocationRepository.GetMockLeaveAllocationRepository();

        var mapperConfig = new MapperConfiguration(c => { c.AddProfile<LeaveAllocationProfile>(); });

        _mapper = mapperConfig.CreateMapper();
        _mockAppLogger = new Mock<IAppLogger<GetLeaveAllocationDetailRequestHandler>>();
    }

    [Fact]
    public async Task GetLeaveAllocationDetailsTest()
    {
        var handler = new GetLeaveAllocationDetailRequestHandler(_mockRepo.Object, _mapper);

        var result = await handler.Handle(new GetLeaveAllocationDetailQuery { Id = 1 }, CancellationToken.None);

        result.ShouldBeOfType<LeaveAllocationDetailsDto>();
        result.Id.ShouldBe(1);
    }

    [Fact]
    public async Task GetLeaveAllocationDetails_NotFoundTest()
    {
        var handler = new GetLeaveAllocationDetailRequestHandler(_mockRepo.Object, _mapper);

        var command = new GetLeaveAllocationDetailQuery { Id = 99 };
        await Should.ThrowAsync<NotFoundException>(async () => await handler.Handle(command, CancellationToken.None));
    }
}