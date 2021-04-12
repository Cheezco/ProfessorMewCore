using Microsoft.EntityFrameworkCore;
using ProfessorMewData.Models;
using ProfessorMewData.Models.Guild;
using System.Collections.Generic;
using System.Configuration;

namespace ProfessorMewData.Contexts
{
    public class GuildContext : DbContext
    {
        public DbSet<GuildRole> Roles { get; set; }
        public DbSet<SavedGuild> Guilds { get; set; }
        public DbSet<SavedLink> Links { get; set; }
        public DbSet<SavedChannel> Channels { get; set; }
        public DbSet<SavedMessage> Messages { get; set; }
        public DbSet<Rank> Ranks { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<LotteryUser> LotteryUsers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = ConfigurationManager.OpenExeConfiguration(System.Reflection.Assembly.GetExecutingAssembly().Location);
            optionsBuilder.UseMySQL(config.ConnectionStrings.ConnectionStrings["GuildConnectionString"].ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SavedGuild>(entity =>
            {
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.DBDiscordID)
                    .IsRequired();

                entity.Property(e => e.Prefix)
                    .HasDefaultValue("!")
                    .IsRequired();
            });

            modelBuilder.Entity<GuildRole>(entity =>
            {
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                 .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .IsRequired();

                entity.Property(e => e.DBDiscordID)
                    .IsRequired();

                entity.Property(e => e.AddUserToDatabase)
                    .IsRequired();

                entity.HasOne(r => (SavedGuild)r.Guild)
                    .WithMany(r => (List<GuildRole>)r.Roles)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity<SavedLink>(entity =>
            {
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .IsRequired();

                entity.Property(e => e.URL)
                    .IsRequired();

                entity.HasOne(r => (SavedGuild)r.Guild)
                    .WithMany(r => (List<SavedLink>)r.Links)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity<SavedChannel>(entity =>
            {
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .IsRequired();

                entity.Property(e => e.DBDiscordID)
                    .IsRequired();

                entity.Property(e => e.ExecuteCommands)
                    .HasDefaultValue(false);

                entity.HasOne(r => (SavedGuild)r.Guild)
                    .WithMany(r => (List<SavedChannel>)r.Channels)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity<Rank>(entity =>
            {
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .IsRequired();

                entity.Property(e => e.RankOrder)
                    .IsRequired();

                entity.Property(e => e.DBDiscordID)
                    .IsRequired();

                entity.Property(e => e.MinPoints)
                    .IsRequired();

                entity.Property(e => e.MaxPoints)
                    .IsRequired();

                entity.Property(e => e.ColorCode)
                    .IsRequired();

                entity.Property(e => e.Default)
                    .IsRequired();

                entity.HasOne(r => (SavedGuild)r.Guild)
                    .WithMany(r => (List<Rank>)r.Ranks)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.DBDiscordID)
                    .IsRequired();

                entity.Property(e => e.TotalPoints)
                    .IsRequired();

                entity.Property(e => e.MonthPoints)
                    .IsRequired();

                entity.Property(e => e.LastUpdate)
                    .IsRequired();

                entity.Property(e => e.PointDisplay)
                    .IsRequired();

                entity.Property(e => e.Privileges)
                    .IsRequired();

                entity.Ignore(e => e.DiscordID);

                entity.HasOne(r => (SavedGuild)r.Guild)
                    .WithMany(r => (List<User>)r.Users)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                entity.HasOne(r => (Rank)r.Rank)
                    .WithMany(r => (List<User>)r.Users)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();
            });

            modelBuilder.Entity<LotteryUser>(entity =>
            {
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.DBDiscordID)
                    .IsRequired();

                entity.Property(e => e.Tickets)
                    .IsRequired();

                entity.HasOne(r => (SavedGuild)r.Guild)
                    .WithMany(r => (List<LotteryUser>)r.LotteryUsers)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                entity.HasOne(r => (User)r.User)
                    .WithOne(r => (LotteryUser)r.LotteryUser)
                    .HasForeignKey<LotteryUser>(r => r.UserID)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity<SavedMessage>(entity =>
            {
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.DBDiscordID)
                    .IsRequired();

                entity.Property(e => e.Name)
                    .IsRequired();

                entity.HasOne(r => (SavedGuild)r.Guild)
                    .WithMany(r => (List<SavedMessage>)r.Messages)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });
        }
    }
}
