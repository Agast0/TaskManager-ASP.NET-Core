using Microsoft.EntityFrameworkCore;
using TaskManager.Models;

namespace TaskManager.Data
{
    public partial class ApiContext: DbContext
    {
        public ApiContext(DbContextOptions<ApiContext> options) : base(options) {}

        public virtual DbSet<Models.Task> Task { get; set; }
        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.Task>(entity =>
            {
                entity.HasKey(t => t.Id);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(t => t.Username);
            });
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
