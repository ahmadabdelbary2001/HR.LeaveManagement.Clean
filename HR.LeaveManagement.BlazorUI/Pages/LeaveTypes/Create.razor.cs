using HR.LeaveManagement.BlazorUI.Contracts;
using HR.LeaveManagement.BlazorUI.Models.LeaveTypes;
using Microsoft.AspNetCore.Components;

namespace HR.LeaveManagement.BlazorUI.Pages.LeaveTypes;

public partial class Create
{
    [Inject] NavigationManager _navManager { get; set; }
    [Inject] ILeaveTypeService _client { get; set; }
    public string Message { get; private set; }

    LeaveTypeVM leaveType = new LeaveTypeVM();

    async Task CreateLeaveType()
    {
        var response = await _client.CreateLeaveType(leaveType);
        if (response.Success)
        {
            _navManager.NavigateTo("/leavetypes/");
        }

        Message = response.Message;
    }
}