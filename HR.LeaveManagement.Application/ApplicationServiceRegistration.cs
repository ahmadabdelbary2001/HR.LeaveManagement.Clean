using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace HR.LeaveManagement.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this  IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddMediatR(Assembly.GetExecutingAssembly());

        return services;
    }
}