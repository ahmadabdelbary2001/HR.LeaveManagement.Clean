using MediatR;

namespace HR.LeaveManagement.Application.Features.LeaveType.Queries.GetAllLeaveTypes;

// public class GetLeaveTypeDetailsQuery : IRequest<List<LeaveTypeDto>>
// {
// }

public record GetLeaveTypesQuery : IRequest<List<LeaveTypeDto>>;