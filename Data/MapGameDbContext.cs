using Microsoft.EntityFrameworkCore;
using InteractiveMapGame.Models;

namespace InteractiveMapGame.Data
{
    public class MapGameDbContext : DbContext
    {
        public MapGameDbContext(DbContextOptions<MapGameDbContext> options) : base(options)
        {
        }

        public DbSet<MapObject> MapObjects { get; set; }
        public DbSet<InteractionLog> InteractionLogs { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<AdminSession> AdminSessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure MapObject
            modelBuilder.Entity<MapObject>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Type).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Category).HasMaxLength(100);
                entity.Property(e => e.Era).HasMaxLength(100);
                entity.Property(e => e.Manufacturer).HasMaxLength(100);
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.ImageUrl).HasMaxLength(200);
                entity.Property(e => e.ModelUrl).HasMaxLength(200);
                entity.Property(e => e.Video360Url).HasMaxLength(200);
                
                // Indexes for performance
                entity.HasIndex(e => e.Type);
                entity.HasIndex(e => e.Category);
                entity.HasIndex(e => e.IsUnlocked);
                entity.HasIndex(e => new { e.X, e.Y, e.Z });
            });

            // Configure PlayerProgress
            modelBuilder.Entity<PlayerProgress>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PlayerId).IsRequired().HasMaxLength(64);
                entity.Property(e => e.SessionId).IsRequired().HasMaxLength(64);
                entity.Property(e => e.UnlockedObjects).HasColumnType("TEXT");
                entity.Property(e => e.CompletedQuests).HasColumnType("TEXT");
                entity.Property(e => e.PlayerPreferences).HasColumnType("TEXT");
                
                entity.HasIndex(e => e.PlayerId);
                entity.HasIndex(e => e.SessionId);
            });

            // Configure InteractionLog
            modelBuilder.Entity<InteractionLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PlayerId).IsRequired().HasMaxLength(64);
                entity.Property(e => e.InteractionType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.InteractionData).HasMaxLength(1000);
                entity.Property(e => e.LLMPrompt).HasMaxLength(2000);
                entity.Property(e => e.LLMResponse).HasMaxLength(2000);
                
                entity.HasIndex(e => e.PlayerId);
                entity.HasIndex(e => e.MapObjectId);
                entity.HasIndex(e => e.Timestamp);
            });

            // Configure Admin
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(200);
                entity.Property(e => e.FullName).HasMaxLength(100);
                
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.IsActive);
            });

            // Configure AdminSession
            modelBuilder.Entity<AdminSession>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SessionToken).IsRequired().HasMaxLength(64);
                entity.Property(e => e.IpAddress).IsRequired().HasMaxLength(45);
                entity.Property(e => e.UserAgent).HasMaxLength(500);
                
                entity.HasIndex(e => e.SessionToken).IsUnique();
                entity.HasIndex(e => e.AdminId);
                entity.HasIndex(e => e.ExpiresAt);
                
                entity.HasOne(e => e.Admin)
                    .WithMany(e => e.Sessions)
                    .HasForeignKey(e => e.AdminId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
