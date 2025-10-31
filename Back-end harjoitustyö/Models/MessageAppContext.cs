using Microsoft.EntityFrameworkCore;

namespace Back_end_harjoitustyö.Models
{
    public class MessageAppContext : DbContext
    {
        public MessageAppContext(DbContextOptions<MessageAppContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.PreviousMessage)
                .WithMany()
                .HasForeignKey(m => m.PreviousMessageId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}