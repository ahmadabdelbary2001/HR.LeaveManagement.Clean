using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Domain;
using Moq;

namespace HR.LeaveManagement.Application.UnitTests.Mocks;

public class MockLeaveAllocationRepository
{
    public static Mock<ILeaveAllocationRepository> GetMockLeaveAllocationRepository()
    {
        var leaveAllocations = new List<LeaveAllocation>
        {
            new LeaveAllocation
            {
                Id = 1,
                NumberOfDays = 10,
                LeaveTypeId = 1,
                Period = 2025,
                EmployeeId = "testuser1",
                LeaveType = new LeaveType { Id = 1, Name = "Test Vacation", DefaultDays = 10 }
            },
            new LeaveAllocation
            {
                Id = 2,
                NumberOfDays = 15,
                LeaveTypeId = 2,
                Period = 2025,
                EmployeeId = "testuser2",
                LeaveType = new LeaveType { Id = 2, Name = "Test Sick", DefaultDays = 15 }
            }
        };

        var mockRepo = new Mock<ILeaveAllocationRepository>();

        mockRepo.Setup(r => r.GetAsync()).ReturnsAsync(leaveAllocations);

        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) => leaveAllocations.FirstOrDefault(la => la.Id == id));

        mockRepo.Setup(r => r.CreateAsync(It.IsAny<LeaveAllocation>()))
            .Returns((LeaveAllocation leaveAllocation) =>
            {
                leaveAllocations.Add(leaveAllocation);
                return Task.CompletedTask;
            });

        mockRepo.Setup(r => r.UpdateAsync(It.IsAny<LeaveAllocation>()))
            .Returns((LeaveAllocation leaveAllocation) =>
            {
                var existing = leaveAllocations.FirstOrDefault(la => la.Id == leaveAllocation.Id);
                if (existing != null)
                {
                    existing.NumberOfDays = leaveAllocation.NumberOfDays;
                    existing.LeaveTypeId = leaveAllocation.LeaveTypeId;
                    existing.Period = leaveAllocation.Period;
                    existing.EmployeeId = leaveAllocation.EmployeeId;
                }
                return Task.CompletedTask;
            });

        mockRepo.Setup(r => r.DeleteAsync(It.IsAny<LeaveAllocation>()))
            .Returns((LeaveAllocation leaveAllocation) =>
            {
                leaveAllocations.Remove(leaveAllocation);
                return Task.CompletedTask;
            });

        mockRepo.Setup(r => r.GetLeaveAllocationsWithDetails()).ReturnsAsync(leaveAllocations);
        mockRepo.Setup(r => r.GetLeaveAllocationWithDetails(It.IsAny<int>()))
            .ReturnsAsync((int id) => leaveAllocations.FirstOrDefault(la => la.Id == id));

        return mockRepo;
    }
}
