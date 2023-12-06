#region

using AgilizAPI.Repositories;
using Microsoft.EntityFrameworkCore;

#endregion

namespace AgilizAPI.Data;

public static class DataExtensions
{
    public static async Task InitializeDb(this IServiceProvider services)
    {
        using var scope     = services.CreateScope();
        var       dbcontext = scope.ServiceProvider.GetRequiredService<AgilizApiContext>();
        await dbcontext.Database.MigrateAsync().ConfigureAwait(false);
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration config)
    {
        var connString = config.GetConnectionString("APIContext");
        services.AddDbContext<AgilizApiContext>(options => options.UseNpgsql(connString))
            .AddScoped<IUsersRepo, UsersRepo>()
            .AddScoped<EstabRepo>()
            .AddScoped<SchedulerRepo>()
            .AddScoped<ServicesRepo>()
            .AddScoped<Populate>();
        return services;
    }
}