#region

using AgilizAPI.Repositories;
using Microsoft.EntityFrameworkCore;

#endregion

namespace AgilizAPI.Data;

public static class DataExtensions
{
    public static async Task<IServiceProvider> InitializeDb(this IServiceProvider services)
    {
        using IServiceScope scope = services.CreateScope();
        await scope.ServiceProvider.GetRequiredService<AgilizApiContext>().Database.MigrateAsync()
            .ConfigureAwait(false);
        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration config)
    {
        string connString =
            "User ID=NashiCodes; Password=5JobQKuMZ1iz; Host=ep-gentle-feather-51998870.us-east-1.aws.neon.tech; Port=5432; Database=agilizappDB; SSL Mode = require;";
        services.AddDbContext<AgilizApiContext>(options => options.UseNpgsql(connString))
            .AddScoped<IUsersRepo, UsersRepo>()
            .AddScoped<EstabRepo>()
            .AddScoped<SchedulerRepo>()
            .AddScoped<ServicesRepo>();
        return services;
    }
}