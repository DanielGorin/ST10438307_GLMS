using Microsoft.EntityFrameworkCore;
using ST10438307_GLMS.Models;
using System.Reflection.Emit;

namespace ST10438307_GLMS.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Client> Clients { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<ServiceRequest> ServiceRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 1 Cleint many Contracts
        modelBuilder.Entity<Contract>()
            .HasOne(c => c.Client)
            .WithMany(cl => cl.Contracts)
            .HasForeignKey(c => c.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        // 1 Contract many ServiceRequests
        modelBuilder.Entity<ServiceRequest>()
            .HasOne(sr => sr.Contract)
            .WithMany(c => c.ServiceRequests)
            .HasForeignKey(sr => sr.ContractId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Contract>()
            .Property(c => c.Status)
            .HasConversion<string>();
    }
}