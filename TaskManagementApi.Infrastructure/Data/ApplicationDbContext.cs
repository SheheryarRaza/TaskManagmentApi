using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Core.Entities;

namespace TaskManagementApi.Core.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<TaskItem> TaskItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TaskItem>()
                .Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(250);

            // Configure 'Description' to have a max length.
            modelBuilder.Entity<TaskItem>()
                .Property(t => t.Description)
                .HasMaxLength(1000);

            // Set default value for IsCompleted.
            modelBuilder.Entity<TaskItem>()
                .Property(t => t.IsCompleted)
                .HasDefaultValue(false);

            // Set default value for CreatedAt.
            modelBuilder.Entity<TaskItem>()
                .Property(t => t.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Set default value for UpdatedAt.
            modelBuilder.Entity<TaskItem>()
                .Property(t => t.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

        }

        public override int SaveChanges()
        {
            UpdateAuditFields();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateAuditFields()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is TaskItem taskItem)
                {
                    if (entry.State == EntityState.Added)
                    {
                        taskItem.CreatedAt = DateTime.UtcNow;
                        taskItem.UpdatedAt = DateTime.UtcNow;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        taskItem.UpdatedAt = DateTime.UtcNow;
                        entry.Property("CreatedAt").IsModified = false;
                    }
                }
            }
        }
    }
}
