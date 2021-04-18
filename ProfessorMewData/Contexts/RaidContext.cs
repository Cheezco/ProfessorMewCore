using Microsoft.EntityFrameworkCore;
using ProfessorMewData.Models.Raid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ProfessorMewData.Contexts
{
    public class RaidContext : DbContext
    {
        public DbSet<RaidUser> Users { get; set; }
        public DbSet<RaidRecord> Records { get; set; }
        public DbSet<RaidGuild> Guilds { get; set; }
        public DbSet<RaidLink> Links { get; set; }
        public DbSet<RaidBench> RaidBenches { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = ConfigurationManager.OpenExeConfiguration(System.Reflection.Assembly.GetExecutingAssembly().Location);
            optionsBuilder.UseMySQL(config.ConnectionStrings.ConnectionStrings["RaidConnectionString"].ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RaidRecord>(entity =>
            {
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.CharacterName)
                    .HasDefaultValue("None")
                    .IsRequired();

                entity.Property(e => e.Class)
                    .IsRequired();

                entity.Property(e => e.Specialization)
                    .IsRequired();

                entity.Property(e => e.Role)
                    .IsRequired();

                entity.Property(e => e.DPS)
                    .IsRequired();

                entity.Property(e => e.BoonUptime)
                    .IsRequired();

                entity.HasOne(r => (RaidUser)r.User)
                    .WithMany(r => (List<RaidRecord>)r.Records)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity<RaidUser>(entity =>
            {
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.AccountName)
                    .HasDefaultValue("None")
                    .IsRequired();

                entity.Property(e => e.DBDiscordID)
                    .IsRequired();

                entity.Ignore(e => e.DiscordID);

                entity.HasOne(r => (RaidGuild)r.Guild)
                    .WithMany(r => (List<RaidUser>)r.Users)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity<RaidLink>(entity =>
            {
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .IsRequired();

                entity.Property(e => e.URL)
                    .IsRequired();

                entity.HasOne(r => (RaidGuild)r.Guild)
                    .WithMany(r => (List<RaidLink>)r.Links)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity<RaidBench>(entity =>
            {
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Class)
                    .IsRequired();

                entity.Property(e => e.Specialization)
                    .IsRequired();

                entity.Property(e => e.Role)
                    .IsRequired();

                entity.Property(e => e.DPS)
                    .IsRequired();

                entity.Property(e => e.BoonUptime)
                    .IsRequired();

                entity.Property(e => e.Scale)
                    .IsRequired();

                entity.HasOne(r => (RaidGuild)r.Guild)
                    .WithMany(r => (List<RaidBench>)r.Benches)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity<RaidGuild>(entity =>
            {
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.DBDiscordID)
                    .IsRequired();

                entity.Ignore(e => e.DiscordID);
            });

        }
    }
}
