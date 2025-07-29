using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Core.Entities;

namespace TaskManagementApi.Core.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<SubTaskItem> SubTaskItems {  get; set; }
        public DbSet<Tag> Tags { get; set; } 
        public DbSet<TaskItemTag> TaskItemTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.AssignedByUser) // A TaskItem has one User (assigner)
                .WithMany()                    // A User can assign many TaskItems
                .HasForeignKey(t => t.AssignedByUserId) // The foreign key property for assigner
                .IsRequired(false)             // AssignedByUserId is optional
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskItem>()
                .Property(t => t.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<TaskItem>()
                .Property(t => t.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<TaskItem>()
               .Property(t => t.IsNotificationEnabled)
               .HasDefaultValue(false);

            modelBuilder.Entity<TaskItem>()
                .Property(t => t.IsNotified)
                .HasDefaultValue(false);

            modelBuilder.Entity<TaskItem>().HasQueryFilter(t => !t.IsDeleted);

            modelBuilder.Entity<TaskItem>()
                .Property(t => t.Priority)
                .HasConversion<string>();

            modelBuilder.Entity<SubTaskItem>()
                .HasOne(st => st.ParentTask) // A SubTaskItem has one ParentTask
                .WithMany(t => t.SubTasks) // A ParentTask can have many SubTasks
                .HasForeignKey(st => st.ParentTaskId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); // If a parent task is deleted, its subtasks are also deleted

            modelBuilder.Entity<SubTaskItem>()
                .HasOne(st => st.User)
                .WithMany()
                .HasForeignKey(st => st.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Configure default values for SubTaskItem audit and soft delete fields
            modelBuilder.Entity<SubTaskItem>()
                .Property(st => st.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<SubTaskItem>()
                .Property(st => st.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<SubTaskItem>()
                .Property(st => st.IsDeleted)
                .HasDefaultValue(false);

            modelBuilder.Entity<SubTaskItem>().HasQueryFilter(st => !st.IsDeleted && !st.ParentTask.IsDeleted);

            modelBuilder.Entity<SubTaskItem>()
               .Property(st => st.Priority)
               .HasConversion<string>(); // Store enum as string in DB

            modelBuilder.Entity<Tag>()
                .HasIndex(t => t.Name)
                .IsUnique();

            modelBuilder.Entity<TaskItemTag>()
                .HasKey(tit => new { tit.TaskItemId, tit.TagId });

            modelBuilder.Entity<TaskItemTag>()
                .HasOne(tit => tit.TaskItem)
                .WithMany(ti => ti.TaskItemTags)
                .HasForeignKey(tit => tit.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskItemTag>()
                .HasOne(tit => tit.Tag)
                .WithMany(t => t.TaskItemTags)
                .HasForeignKey(tit => tit.TagId)
                .OnDelete(DeleteBehavior.Restrict);
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
                        taskItem.IsDeleted = false;
                        taskItem.DeletedAt = null;
                        taskItem.IsNotified = false;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        taskItem.UpdatedAt = DateTime.UtcNow;

                        if (entry.Property(nameof(TaskItem.IsDeleted)).IsModified && taskItem.IsDeleted)
                        {
                            taskItem.DeletedAt = DateTime.UtcNow;
                        }
                        else if (entry.Property(nameof(TaskItem.IsDeleted)).IsModified && !taskItem.IsDeleted)
                        {
                            taskItem.DeletedAt = null;
                        }
                        entry.Property("CreatedAt").IsModified = false;

                        if (entry.Property(nameof(TaskItem.IsNotified)).IsModified == false)
                        {
                            entry.Property(nameof(TaskItem.IsNotified)).IsModified = false;
                        }
                    }
                    else if (entry.State == EntityState.Deleted)
                    {
                        entry.State = EntityState.Modified;
                        taskItem.IsDeleted = true;
                        taskItem.DeletedAt = DateTime.UtcNow;
                        taskItem.IsNotified = false;
                    }
                }
                else if (entry.Entity is SubTaskItem subTaskItem)
                {
                    if (entry.State == EntityState.Added)
                    {
                        subTaskItem.CreatedAt = DateTime.UtcNow;
                        subTaskItem.UpdatedAt = DateTime.UtcNow;
                        subTaskItem.IsDeleted = false;
                        subTaskItem.DeletedAt = null;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        subTaskItem.UpdatedAt = DateTime.UtcNow;
                        if (entry.Property(nameof(SubTaskItem.IsDeleted)).IsModified && subTaskItem.IsDeleted)
                        {
                            subTaskItem.DeletedAt = DateTime.UtcNow;
                        }
                        else if (entry.Property(nameof(SubTaskItem.IsDeleted)).IsModified && !subTaskItem.IsDeleted)
                        {
                            subTaskItem.DeletedAt = null;
                        }
                        entry.Property("CreatedAt").IsModified = false;
                    }
                    else if (entry.State == EntityState.Deleted)
                    {
                        entry.State = EntityState.Modified;
                        subTaskItem.IsDeleted = true;
                        subTaskItem.DeletedAt = DateTime.UtcNow;
                    }
                }
            }
        }
    }
}
