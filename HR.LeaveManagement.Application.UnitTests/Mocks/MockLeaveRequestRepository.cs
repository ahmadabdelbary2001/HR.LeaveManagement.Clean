using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Domain;
using Moq;

namespace HR.LeaveManagement.Application.UnitTests.Mocks;

public class MockLeaveRequestRepository
{
    public static Mock<ILeaveRequestRepository> GetMockLeaveRequestRepository()
    {
        var leaveRequests = new List<LeaveRequest>
        {
            new LeaveRequest
            {
                Id = 1,
                StartDate = DateTime.Now.AddDays(-5),
                EndDate = DateTime.Now.AddDays(5),
                LeaveTypeId = 1,
                RequestingEmployeeId = "testuser1",
                Approved = true,
                Cancelled = false,
                RequestComments = "Test Request 1",
                LeaveType = new LeaveType { Id = 1, Name = "Test Vacation", DefaultDays = 10 }
            },
            new LeaveRequest
            {
                Id = 2,
                StartDate = DateTime.Now.AddDays(-10),
                EndDate = DateTime.Now.AddDays(-2),
                LeaveTypeId = 2,
                RequestingEmployeeId = "testuser2",
                Approved = false,
                Cancelled = false,
                RequestComments = "Test Request 2",
                LeaveType = new LeaveType { Id = 2, Name = "Test Sick", DefaultDays = 15 }
            }
        };

        var mockRepo = new Mock<ILeaveRequestRepository>();

        mockRepo.Setup(r => r.GetAsync()).ReturnsAsync(leaveRequests);

        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) => leaveRequests.FirstOrDefault(lr => lr.Id == id));

        mockRepo.Setup(r => r.CreateAsync(It.IsAny<LeaveRequest>()))
            .Returns((LeaveRequest leaveRequest) =>
            {
                leaveRequests.Add(leaveRequest);
                return Task.CompletedTask;
            });

        mockRepo.Setup(r => r.UpdateAsync(It.IsAny<LeaveRequest>()))
            .Returns((LeaveRequest leaveRequest) =>
            {
                var existing = leaveRequests.FirstOrDefault(lr => lr.Id == leaveRequest.Id);
                if (existing != null)
                {
                    existing.StartDate = leaveRequest.StartDate;
                    existing.EndDate = leaveRequest.EndDate;
                    existing.LeaveTypeId = leaveRequest.LeaveTypeId;
                    existing.RequestingEmployeeId = leaveRequest.RequestingEmployeeId;
                    existing.Approved = leaveRequest.Approved;
                    existing.Cancelled = leaveRequest.Cancelled;
                    existing.RequestComments = leaveRequest.RequestComments;
                }
                return Task.CompletedTask;
            });

        mockRepo.Setup(r => r.DeleteAsync(It.IsAny<LeaveRequest>()))
            .Returns((LeaveRequest leaveRequest) =>
            {
                leaveRequests.Remove(leaveRequest);
                return Task.CompletedTask;
            });

        mockRepo.Setup(r => r.GetLeaveRequestsWithDetails()).ReturnsAsync(leaveRequests);
        mockRepo.Setup(r => r.GetLeaveRequestWithDetails(It.IsAny<int>()))
            .ReturnsAsync((int id) => leaveRequests.FirstOrDefault(lr => lr.Id == id));

        return mockRepo;
    }
}
