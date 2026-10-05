using entra_auth_lab.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace entra_auth_lab.Api.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.ExternalId).IsUnique();
                entity.Property(e => e.Email).HasMaxLength(256);
                entity.Property(e => e.DisplayName).HasMaxLength(200);
                // Seed Test Data
                entity.HasData(
                    new User
                    {
                        Id = 1, // Explicitly provide the PK for seed data
                        ExternalId = new Guid("11111111-1111-1111-1111-111111111111"),
                        Email = "testuser1@example.com",
                        DisplayName = "Test User One",
                        CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    },
                    new User
                    {
                        Id = 2,
                        ExternalId = new Guid("22222222-2222-2222-2222-222222222222"),
                        Email = "testuser2@example.com",
                        DisplayName = "Test User Two",
                        CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    }
                );
            });
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var currentTime = DateTime.UtcNow;
            // Find every entity tracked by EF Core that implements IAuditable
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is IAuditable && e.State == EntityState.Added);
            foreach (var entry in entries)
            {
                var auditable = (IAuditable)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    auditable.CreatedAt = currentTime;
                }
            }
            // Call the original, built-in EF Core save logic to commit to the database
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}