using Microsoft.EntityFrameworkCore;
using Planpipe.Core.Models;

namespace Planpipe.Infrastructure.Data;

public class PlanpipeDbContext : DbContext
{
    public PlanpipeDbContext(DbContextOptions<PlanpipeDbContext> options)
        : base(options)
    {
    }

    public DbSet<ProcessingRun> ProcessingRuns => Set<ProcessingRun>();
    public DbSet<PlanFeature> PlanFeatures => Set<PlanFeature>();
    public DbSet<QuantityItem> QuantityItems => Set<QuantityItem>();
    public DbSet<GroundTruthItem> GroundTruthItems => Set<GroundTruthItem>();
    public DbSet<ComparisonResult> ComparisonResults => Set<ComparisonResult>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProcessingRun>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.ErrorMessage).HasMaxLength(2000);
            
            entity.HasMany(e => e.Features)
                .WithOne(e => e.ProcessingRun)
                .HasForeignKey(e => e.ProcessingRunId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasMany(e => e.QuantityItems)
                .WithOne(e => e.ProcessingRun)
                .HasForeignKey(e => e.ProcessingRunId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasMany(e => e.ComparisonResults)
                .WithOne(e => e.ProcessingRun)
                .HasForeignKey(e => e.ProcessingRunId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PlanFeature>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Value).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Unit).HasMaxLength(50);
        });

        modelBuilder.Entity<QuantityItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ItemCode).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Quantity).HasPrecision(18, 4);
            entity.Property(e => e.Unit).IsRequired().HasMaxLength(50);
        });

        modelBuilder.Entity<GroundTruthItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ItemCode).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Quantity).HasPrecision(18, 4);
            entity.Property(e => e.Unit).IsRequired().HasMaxLength(50);
        });

        modelBuilder.Entity<ComparisonResult>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ItemCode).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ExtractedQuantity).HasPrecision(18, 4);
            entity.Property(e => e.GroundTruthQuantity).HasPrecision(18, 4);
        });
    }
}
