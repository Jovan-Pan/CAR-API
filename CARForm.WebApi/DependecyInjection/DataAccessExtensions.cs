using Contracts.Repository;
using Repository;

namespace WebApi.DependecyInjection;

public static class DataAccessExtensions
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services)
    {
        services.AddSingleton<DbContext>();
        services.AddScoped<IDataManager, RepositoryManager>();
        return services;
    }
}
