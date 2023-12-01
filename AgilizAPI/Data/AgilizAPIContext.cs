#region

using AgilizAPI.Models;
using Microsoft.EntityFrameworkCore;

#endregion

namespace AgilizAPI.Data;

public class AgilizApiContext : DbContext
{
    public AgilizApiContext(DbContextOptions<AgilizApiContext> options)
        : base(options) { }

    public DbSet<User> User { get; set; } = default!;
    public DbSet<Establishment> Establishment { get; set; } = default!;
    public DbSet<Services> Services { get; set; } = default!;
    public DbSet<Scheduler> Scheduler { get; set; } = default!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("Users").HasKey(u => u.Email);
        modelBuilder.Entity<Establishment>().ToTable("Establishments").HasKey(e => e.Id);
        modelBuilder.Entity<Services>().ToTable("Services").HasKey(s => s.Id);
        modelBuilder.Entity<Scheduler>().ToTable("Scheduler").HasKey(s => s.Id);
    }
}