using AgilizAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AgilizAPI.Data;

public static class DataExtensions
{
    public static async Task InitializeDb(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbcontext = scope.ServiceProvider.GetRequiredService<APIContext>();
        await dbcontext.Database.MigrateAsync();
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration config)
    {
        var connString = config.GetConnectionString("APIContext");
        services.AddNpgsql<APIContext>(connString).AddScoped<IUsersRepo, UsersRepo>();
        return services;
    }
}