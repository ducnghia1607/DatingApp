using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {

    }

    public DbSet<AppUser> Users { set; get; }

    public DbSet<Photo> Photos { set; get; }

    public DbSet<UserLike> Likes { set; get; }

    public DbSet<Message> Messages { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserLike>()
        .HasKey(l => new { l.SourceUserId, l.TargetUserId });

        // 1 person can like many one  
        // foreign key is  field (or collection of fields) in one table, that refers to the PRIMARY KEY in another tabl
        modelBuilder.Entity<UserLike>()
        .HasOne(l => l.SourceUser)
        .WithMany(u => u.LikedUsers)
        .HasForeignKey(l => l.SourceUserId)
        .OnDelete(DeleteBehavior.Cascade);

        // 1 nguoi co the duoc nhieu nguoi like 
        modelBuilder.Entity<UserLike>()
        .HasOne(l => l.TargetUser)
        .WithMany(u => u.LikedByUsers)
        .HasForeignKey(l => l.TargetUserId)
        .OnDelete(DeleteBehavior.NoAction);


        modelBuilder.Entity<Message>()
        .HasOne(m => m.Sender)
        .WithMany(u => u.SentMessages)
        .HasForeignKey(m => m.SenderId)
        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Message>()
         .HasOne(m => m.Recipient)
         .WithMany(u => u.ReceivedMessages)
         .HasForeignKey(m => m.RecipientId)
         .OnDelete(DeleteBehavior.Restrict);

    }

}

