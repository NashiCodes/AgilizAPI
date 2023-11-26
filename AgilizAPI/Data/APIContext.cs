using AgilizAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AgilizAPI.Data;

public class APIContext : DbContext
{
    public APIContext(DbContextOptions<APIContext> options)
        : base(options)
    {
    }

    public DbSet<User> User => Set<User>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("Users");
    }
}