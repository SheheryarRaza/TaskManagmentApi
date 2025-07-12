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
                .HasOne(t => t.User) // A TaskItem has one User
                .WithMany()          // A User can have many TaskItems (no navigation property back to tasks on User)
                .HasForeignKey(t => t.UserId) // The foreign key property
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskItem>()
                .Property(t => t.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

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
