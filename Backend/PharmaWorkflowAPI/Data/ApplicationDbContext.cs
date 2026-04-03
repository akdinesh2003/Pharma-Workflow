using Microsoft.EntityFrameworkCore;
using PharmaWorkflowAPI.Models;

namespace PharmaWorkflowAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<MainTransaction> Main_Transactions { get; set; }
    public DbSet<HistoryTransaction> History_Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Main_Transactions
        modelBuilder.Entity<MainTransaction>(entity =>
        {
            entity.ToTable("Main_Transactions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RowVersion).IsRowVersion();
            entity.HasIndex(e => e.BatchNo).IsUnique();
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.IsDeleted);
        });

        // Configure History_Transactions
        modelBuilder.Entity<HistoryTransaction>(entity =>
        {
            entity.ToTable("History_Transactions");
            entity.HasKey(e => e.HistoryId);
            entity.HasOne(e => e.Transaction)
                  .WithMany(t => t.History)
                  .HasForeignKey(e => e.TransactionId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.TransactionId);
        });

        // Configure Users
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique();
        });
    }
}
