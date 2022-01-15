using Microsoft.EntityFrameworkCore;
using Guestbook.Models;

namespace Guestbook.Models
{
    public class GuestbookDBContext : DbContext
    {
        public GuestbookDBContext(DbContextOptions<GuestbookDBContext> option):base(option)
        {
                
        }
        public virtual DbSet<User> Users { get; set; }
       
        public virtual DbSet<Message> Message { get; set; }
        public virtual DbSet<UserMessage> UserMessages { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserMessage>().HasKey(n => new { n.UserId, n.MessId });
            modelBuilder.Entity<UserMessage>()
                .HasOne(n => n.Users)
                .WithMany(n => n.UserMessages)
                .HasForeignKey(n => n.UserId);
            modelBuilder.Entity<UserMessage>()
                .HasOne(n => n.Messages)
                .WithMany(n => n.UserMessages)
                .HasForeignKey(n => n.MessId);
           
            modelBuilder.Entity<User>().HasIndex(n => n.Username).IsUnique();
        }
    }
}
