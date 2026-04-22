// EFcore Dbcontext registers tables and configures relationships

using Microsoft.EntityFrameworkCore;
using ST10438307_GLMS.Models;

namespace ST10438307_GLMS.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    //Tables
    //-----------------------------------------------------------------------------------------------
    public DbSet<Client> Clients { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<ServiceRequest> ServiceRequests { get; set; }
    //-----------------------------------------------------------------------------------------------

    //Relationships and constraints
    //-----------------------------------------------------------------------------------------------
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // one clientmany contracts - removing a client removes their contracts
        modelBuilder.Entity<Contract>()
            .HasOne(c => c.Client)
            .WithMany(cl => cl.Contracts)
            .HasForeignKey(c => c.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        // one contract many service requests
        modelBuilder.Entity<ServiceRequest>()
            .HasOne(sr => sr.Contract)
            .WithMany(c => c.ServiceRequests)
            .HasForeignKey(sr => sr.ContractId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Contract>()
            .Property(c => c.Status)
            .HasConversion<string>();

        modelBuilder.Entity<ServiceRequest>()
            .Property(sr => sr.Status)
            .HasConversion<string>();
    }
    //-----------------------------------------------------------------------------------------------
}