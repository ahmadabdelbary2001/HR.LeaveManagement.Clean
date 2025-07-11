namespace HR.LeaveManagement.BlazorUI.Models.LeaveRequest;

public class AdminLeaveRequestViewVM
{
    public int TotalRequests { get; set; }
    public int ApprovedRequests { get; set; }
    public int PendingRequests { get; set; }
    public int RejectedRequests { get; set; }
    public List<LeaveRequestVM> LeaveRequests { get; set; } = new List<LeaveRequestVM>();
}