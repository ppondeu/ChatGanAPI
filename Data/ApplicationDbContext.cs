using Microsoft.EntityFrameworkCore;
using ChatApi.Entities;

namespace ChatApi.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public required DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                // Configure Id to be automatically generated as a GUID
                entity.HasKey(user => user.Id);
                entity.Property(user => user.Id).ValueGeneratedOnAdd().HasDefaultValueSql("newsequentialid()");

                // Configure Username to be unique and required
                entity.Property(user => user.Username).IsRequired();
                entity.HasIndex(user => user.Username).IsUnique();

                // Configure Email to be unique and required
                entity.Property(user => user.Email).IsRequired();
                entity.HasIndex(user => user.Email).IsUnique();

                // Configure Password to be required
                entity.Property(user => user.Password).IsRequired();

                // Configure FirstName and LastName as nullable strings
                entity.Property(user => user.FirstName).IsRequired(false);
                entity.Property(user => user.LastName).IsRequired(false);

                // Configure CreatedAt and UpdatedAt to be auto-set by the application
                entity.Property(user => user.CreatedAt).HasDefaultValueSql("getutcdate()");
                entity.Property(user => user.UpdatedAt).HasDefaultValueSql("getutcdate()");

                // Configure RefreshToken and Avatar as nullable strings
                entity.Property(user => user.RefreshToken).IsRequired(false);
                entity.Property(user => user.Avatar).IsRequired(false);

                // Configure DeletedAt as nullable DateTime (soft delete)
                entity.Property(user => user.DeletedAt).IsRequired(false);
            });
        }
    }
}
