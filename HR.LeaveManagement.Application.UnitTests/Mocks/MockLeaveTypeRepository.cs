using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Domain;
using Moq;

namespace HR.LeaveManagement.Application.UnitTests.Mocks;

public class MockLeaveTypeRepository
{
    public static Mock<ILeaveTypeRepository> GetMockLeaveTypeRepository()
    {
        var leaveTypes = new List<LeaveType>
        {
            new LeaveType
            {
                Id = 1,
                DefaultDays = 10,
                Name = "Test Vacation"
            },
            new LeaveType
            {
                Id = 2,
                DefaultDays = 15,
                Name = "Test Sick"
            },
            new LeaveType
            {
                Id = 3,
                DefaultDays = 15,
                Name = "Test Maternity"
            }
        };

        var mockRepo = new Mock<ILeaveTypeRepository>();

        mockRepo.Setup(r => r.GetAsync()).ReturnsAsync(leaveTypes);

        mockRepo.Setup(r => r.CreateAsync(It.IsAny<LeaveType>()))
            .Returns((LeaveType leaveType) =>
            {
                leaveType.Id = leaveTypes.Count + 1; // Simulate ID generation
                leaveTypes.Add(leaveType);
                return Task.CompletedTask;
            });

        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) => leaveTypes.FirstOrDefault(lt => lt.Id == id));

        mockRepo.Setup(r => r.DeleteAsync(It.IsAny<LeaveType>()))
            .Returns((LeaveType leaveType) =>
            {
                leaveTypes.Remove(leaveType);
                return Task.CompletedTask;
            });

        mockRepo.Setup(r => r.UpdateAsync(It.IsAny<LeaveType>()))
            .Returns((LeaveType leaveType) =>
            {
                var existing = leaveTypes.FirstOrDefault(lt => lt.Id == leaveType.Id);
                if (existing != null)
                {
                    existing.Name = leaveType.Name;
                    existing.DefaultDays = leaveType.DefaultDays;
                }
                return Task.CompletedTask;
            });

        mockRepo.Setup(r => r.IsLeaveTypeUnique(It.IsAny<string>()))
            .ReturnsAsync((string name) => !leaveTypes.Any(lt => lt.Name == name));

        return mockRepo;
    }
}