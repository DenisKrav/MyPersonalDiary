using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyPersonalDiary.DAL.Models;
using MyPersonalDiary.DAL.Models.Identities;

namespace MyPersonalDiary.DAL.ApplicationDbContext
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, long>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Invite> Invites { get; set; }
        public DbSet<DiaryRecord> DiaryRecords { get; set; }
        public DbSet<DiaryImage> DiaryImages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<DiaryRecord>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.EncryptedContent)
                      .IsRequired();

                entity.HasOne(x => x.User)
                      .WithMany()
                      .HasForeignKey(x => x.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<DiaryImage>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.FileName)
                      .IsRequired();

                entity.Property(x => x.ContentType)
                      .IsRequired();

                entity.Property(x => x.Data)
                      .IsRequired();

                entity.HasOne(x => x.Record)
                      .WithMany(r => r.Images)
                      .HasForeignKey(x => x.RecordId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Invite>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Email).IsRequired();
                entity.Property(x => x.Code).IsRequired();
            });
        }
    }
}
