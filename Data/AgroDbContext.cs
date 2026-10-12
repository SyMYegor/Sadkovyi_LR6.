using AgroMaintenance.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AgroMaintenance.Api.Data;

// Завдання 5. Три таблиці in-memory БД.
public sealed class AgroDbContext(
    DbContextOptions<AgroDbContext> options) : DbContext(options)
{
    public DbSet<Device> Devices => Set<Device>();

    public DbSet<WorkType> WorkTypes => Set<WorkType>();

    public DbSet<Maintenance> Maintenances => Set<Maintenance>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Maintenance>()
            .HasOne<Device>()
            .WithMany()
            .HasForeignKey(item => item.DeviceId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Maintenance>()
            .HasOne<WorkType>()
            .WithMany()
            .HasForeignKey(item => item.WorkTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}