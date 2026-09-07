using GitHubIntegration.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GitHubIntegration.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<RepositoryEntity> Repositories => Set<RepositoryEntity>();
    public DbSet<IssueEntity> Issues => Set<IssueEntity>();
    public DbSet<PullRequestEntity> PullRequests => Set<PullRequestEntity>();
    public DbSet<CommitEntity> Commits => Set<CommitEntity>();
    public DbSet<SyncHistoryEntity> SyncHistory => Set<SyncHistoryEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RepositoryEntity>(entity =>
        {
            entity.ToTable("repositories");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.GitHubId).IsUnique();
            entity.HasIndex(x => x.FullName).IsUnique();
            entity.Property(x => x.Owner).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            entity.Property(x => x.FullName).HasMaxLength(210).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(2000);
            entity.Property(x => x.DefaultBranch).HasMaxLength(255).IsRequired();
        });

        modelBuilder.Entity<IssueEntity>(entity =>
        {
            entity.ToTable("issues");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.RepositoryId, x.Number }).IsUnique();
            entity.HasIndex(x => new { x.RepositoryId, x.GitHubId }).IsUnique();
            entity.Property(x => x.Title).HasMaxLength(1000).IsRequired();
            entity.Property(x => x.AuthorLogin).HasMaxLength(100);
            entity.Property(x => x.State).HasConversion<string>().HasMaxLength(20).IsRequired();
            entity.HasOne(x => x.Repository)
                .WithMany(x => x.Issues)
                .HasForeignKey(x => x.RepositoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PullRequestEntity>(entity =>
        {
            entity.ToTable("pull_requests");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.RepositoryId, x.Number }).IsUnique();
            entity.HasIndex(x => new { x.RepositoryId, x.GitHubId }).IsUnique();
            entity.Property(x => x.Title).HasMaxLength(1000).IsRequired();
            entity.Property(x => x.AuthorLogin).HasMaxLength(100);
            entity.Property(x => x.SourceBranch).HasMaxLength(255).IsRequired();
            entity.Property(x => x.TargetBranch).HasMaxLength(255).IsRequired();
            entity.Property(x => x.State).HasConversion<string>().HasMaxLength(20).IsRequired();
            entity.HasOne(x => x.Repository)
                .WithMany(x => x.PullRequests)
                .HasForeignKey(x => x.RepositoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CommitEntity>(entity =>
        {
            entity.ToTable("commits");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.RepositoryId, x.Sha }).IsUnique();
            entity.Property(x => x.Sha).HasMaxLength(64).IsRequired();
            entity.Property(x => x.Branch).HasMaxLength(255).IsRequired();
            entity.Property(x => x.Message).HasMaxLength(8000).IsRequired();
            entity.Property(x => x.AuthorName).HasMaxLength(255);
            entity.Property(x => x.AuthorEmail).HasMaxLength(320);
            entity.Property(x => x.AuthorLogin).HasMaxLength(100);
            entity.HasOne(x => x.Repository)
                .WithMany(x => x.Commits)
                .HasForeignKey(x => x.RepositoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SyncHistoryEntity>(entity =>
        {
            entity.ToTable("sync_history");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.RepositoryId, x.StartedAt });
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
            entity.Property(x => x.ErrorMessage).HasMaxLength(2000);
            entity.HasOne(x => x.Repository)
                .WithMany(x => x.SyncHistory)
                .HasForeignKey(x => x.RepositoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
