using HR.LeaveManagement.BlazorUI.Models.LeaveRequest;
using HR.LeaveManagement.BlazorUI.Services.Base;

namespace HR.LeaveManagement.BlazorUI.Contracts;

public interface ILeaveRequestService
{
    Task<AdminLeaveRequestViewVM> GetAdminLeaveRequestList();
    Task<EmployeeLeaveRequestViewVM> GetUserLeaveRequests();
    Task<Response<Guid>> CreateLeaveRequest(LeaveRequestVM leaveRequest);
    Task<LeaveRequestVM> GetLeaveRequest(int id);
    Task<Response<Guid>> DeleteLeaveRequest(int id);
    Task<Response<Guid>> ApproveLeaveRequest(int id, bool approved);
    Task<Response<Guid>> CancelLeaveRequest(int id);
}