using Microsoft.EntityFrameworkCore;
using todo_list.Models;

namespace todo_list.Data
{
    public class ToDoContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<TodoItem> TodoItems { get; set; }

        public ToDoContext(DbContextOptions<ToDoContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.\\SQLExpress;Database=ToDoListDB;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=False");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id); 

            modelBuilder.Entity<TodoItem>()
                .HasOne(t => t.User) 
                .WithMany(u => u.TodoItems)
                .HasForeignKey(t => t.UserId);
        }
    }
}
