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
    public DbSet<Service> Services { get; set; } = default!;
    public DbSet<Scheduler> Scheduler { get; set; } = default!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UserRequires();
        modelBuilder.EstablishmentRequires();
        modelBuilder.ServiceRequires();
        modelBuilder.SchedulerRequires();
    }
}

public static class ModelBuilderExtensions
{
    public static ModelBuilder UserRequires(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("Users").HasKey(u => u.Email);
        modelBuilder.Entity<User>().Property(e => e.Email).IsRequired();
        modelBuilder.Entity<User>().Property(e => e.Password).IsRequired();
        modelBuilder.Entity<User>().Property(e => e.PasswordSalt).IsRequired();
        modelBuilder.Entity<User>().Property(e => e.UserName).IsRequired();
        modelBuilder.Entity<User>().Property(e => e.UserPhone).IsRequired();
        modelBuilder.Entity<User>().Property(e => e.UserCpf).IsRequired();
        modelBuilder.Entity<User>().Property(e => e.UserAddress).IsRequired();
        modelBuilder.Entity<User>().Property(e => e.addressNumber).IsRequired();
        modelBuilder.Entity<User>().Property(e => e.Role).IsRequired();

        return modelBuilder;
    }

    public static ModelBuilder EstablishmentRequires(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Establishment>().ToTable("Establishments").HasKey(e => e.Id);
        modelBuilder.Entity<Establishment>().Property(e => e.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<Establishment>().Property(e => e.Email).IsRequired();
        modelBuilder.Entity<Establishment>().Property(e => e.Password).IsRequired();
        modelBuilder.Entity<Establishment>().Property(e => e.Name).IsRequired();
        modelBuilder.Entity<Establishment>().Property(e => e.Category).IsRequired();
        modelBuilder.Entity<Establishment>().Property(e => e.Address).IsRequired();
        modelBuilder.Entity<Establishment>().Property(e => e.AddressNumber).IsRequired();
        modelBuilder.Entity<Establishment>().Ignore(e => e.addressJson);

        return modelBuilder;
    }

    public static ModelBuilder ServiceRequires(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Service>().ToTable("Services").HasKey(s => s.Id);
        modelBuilder.Entity<Service>().Property(s => s.Id).ValueGeneratedOnAdd().IsRequired();
        modelBuilder.Entity<Service>().Property(s => s.Name).IsRequired();
        modelBuilder.Entity<Service>().Property(s => s.Description).IsRequired();
        modelBuilder.Entity<Service>().Property(s => s.Price).IsRequired();
        modelBuilder.Entity<Service>().Property(s => s.Duration).IsRequired();

        return modelBuilder;
    }

    public static ModelBuilder SchedulerRequires(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Scheduler>().ToTable("Scheduler").HasKey(s => s.Id);
        modelBuilder.Entity<Scheduler>().Property(s => s.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<Scheduler>().Property(s => s.IdService).IsRequired();
        modelBuilder.Entity<Scheduler>().Property(s => s.IdUser).IsRequired();
        modelBuilder.Entity<Scheduler>().Property(s => s.Date).IsRequired();
        modelBuilder.Entity<Scheduler>().Property(s => s.Hour).IsRequired();

        return modelBuilder;
    }
}