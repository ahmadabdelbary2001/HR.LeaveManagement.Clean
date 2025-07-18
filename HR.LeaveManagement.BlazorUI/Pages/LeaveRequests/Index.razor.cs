using HR.LeaveManagement.BlazorUI.Contracts;
using HR.LeaveManagement.BlazorUI.Models.LeaveRequest;
using Microsoft.AspNetCore.Components;

namespace HR.LeaveManagement.BlazorUI.Pages.LeaveRequests;

public partial class Index
{
    [Inject] ILeaveRequestService leaveRequestService { get; set; }
    [Inject] NavigationManager NavigationManager { get; set; }
    public AdminLeaveRequestViewVM Model { get; set; } = new();

    protected async override Task OnInitializedAsync()
    {
        Model = await leaveRequestService.GetAdminLeaveRequestList();
    }

    void GoToDetails(int id)
    {
        NavigationManager.NavigateTo($"/leaverequests/details/{id}");
    }
}