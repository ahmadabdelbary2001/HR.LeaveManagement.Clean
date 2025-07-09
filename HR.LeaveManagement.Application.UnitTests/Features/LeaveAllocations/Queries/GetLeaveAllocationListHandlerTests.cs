using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Logging;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Features.LeaveAllocation.Queries.GetLeaveAllocations;
using HR.LeaveManagement.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using Moq;
using Shouldly;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveAllocations.Queries;

public class GetLeaveAllocationListHandlerTests
{
    private readonly Mock<ILeaveAllocationRepository> _mockRepo;
    private IMapper _mapper;
    private Mock<IAppLogger<GetLeaveAllocationListHandler>> _mockAppLogger;

    public GetLeaveAllocationListHandlerTests()
    {
        _mockRepo = MockLeaveAllocationRepository.GetMockLeaveAllocationRepository();

        var mapperConfig = new MapperConfiguration(c => { c.AddProfile<LeaveAllocationProfile>(); });

        _mapper = mapperConfig.CreateMapper();
        _mockAppLogger = new Mock<IAppLogger<GetLeaveAllocationListHandler>>();
    }

    [Fact]
    public async Task GetLeaveAllocationListTest()
    {
        var handler = new GetLeaveAllocationListHandler(_mockRepo.Object, _mapper);

        var result = await handler.Handle(new GetLeaveAllocationListQuery(), CancellationToken.None);

        result.ShouldBeOfType<List<LeaveAllocationDto>>();
        result.Count.ShouldBe(2);
    }
}