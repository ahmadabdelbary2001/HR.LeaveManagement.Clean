using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Logging;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveRequest.Commands.DeleteLeaveRequest;
using HR.LeaveManagement.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using Moq;
using Shouldly;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveRequests.Commands;

public class DeleteLeaveRequestHandlerTests
{
    private readonly Mock<ILeaveRequestRepository> _mockLeaveRequestRepo;
    private IMapper _mapper;
    private Mock<IAppLogger<DeleteLeaveRequestCommandHandler>> _mockAppLogger;

    public DeleteLeaveRequestHandlerTests()
    {
        _mockLeaveRequestRepo = MockLeaveRequestRepository.GetMockLeaveRequestRepository();

        var mapperConfig = new MapperConfiguration(c => { c.AddProfile<LeaveRequestProfile>(); });

        _mapper = mapperConfig.CreateMapper();
        _mockAppLogger = new Mock<IAppLogger<DeleteLeaveRequestCommandHandler>>();
    }

    [Fact]
    public async Task DeleteLeaveRequestTest()
    {
        var handler = new DeleteLeaveRequestCommandHandler(
            _mockLeaveRequestRepo.Object);

        var command = new DeleteLeaveRequestCommand { Id = 1 };
        await handler.Handle(command, CancellationToken.None);

        var leaveRequest = await _mockLeaveRequestRepo.Object.GetByIdAsync(1);
        leaveRequest.ShouldBeNull();
    }

    [Fact]
    public async Task DeleteLeaveRequest_NotFoundTest()
    {
        var handler = new DeleteLeaveRequestCommandHandler(
            _mockLeaveRequestRepo.Object);

        var command = new DeleteLeaveRequestCommand { Id = 99 };
        await Should.ThrowAsync<NotFoundException>(async () => await handler.Handle(command, CancellationToken.None));
    }
}