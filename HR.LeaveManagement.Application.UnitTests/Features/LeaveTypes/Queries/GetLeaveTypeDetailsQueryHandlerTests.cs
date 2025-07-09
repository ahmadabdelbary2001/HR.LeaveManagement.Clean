using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Logging;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveType.Queries.GetLeaveTypeDetails;
using HR.LeaveManagement.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using Moq;
using Shouldly;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveTypes.Queries;

public class GetLeaveTypeDetailsQueryHandlerTests
{
    private readonly Mock<ILeaveTypeRepository> _mockRepo;
    private IMapper _mapper;
    private Mock<IAppLogger<GetLeaveTypeDetailsQueryHandler>> _mockAppLogger;

    public GetLeaveTypeDetailsQueryHandlerTests()
    {
        _mockRepo = MockLeaveTypeRepository.GetMockLeaveTypeRepository();

        var mapperConfig = new MapperConfiguration(c => { c.AddProfile<LeaveTypeProfile>(); });

        _mapper = mapperConfig.CreateMapper();
        _mockAppLogger = new Mock<IAppLogger<GetLeaveTypeDetailsQueryHandler>>();
    }

    [Fact]
    public async Task GetLeaveTypeDetailsTest()
    {
        var handler = new GetLeaveTypeDetailsQueryHandler(_mapper, _mockRepo.Object);

        var result = await handler.Handle(new GetLeaveTypeDetailsQuery(1), CancellationToken.None);

        result.ShouldBeOfType<LeaveTypeDetailsDto>();
        result.Id.ShouldBe(1);
    }

    [Fact]
    public async Task GetLeaveTypeDetails_NotFoundTest()
    {
        var handler = new GetLeaveTypeDetailsQueryHandler(_mapper, _mockRepo.Object);

        var command = new GetLeaveTypeDetailsQuery(99);
        await Should.ThrowAsync<NotFoundException>(async () => await handler.Handle(command, CancellationToken.None));
    }
}