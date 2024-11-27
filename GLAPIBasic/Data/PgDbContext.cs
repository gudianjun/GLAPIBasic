using System;
using System.Collections.Generic;
using GLAPIBasic.Models;
using Microsoft.EntityFrameworkCore;

namespace GLAPIBasic.Data;

public partial class PgDbContext : DbContext
{
    public PgDbContext()
    {
    }

    public PgDbContext(DbContextOptions<PgDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<UserHistory> UserHistories { get; set; }

    public virtual DbSet<UserInfo> UserInfos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserHistory>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.SeqNum }).HasName("user_history_pkey");

            entity.ToTable("user_history");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.SeqNum).HasColumnName("seq_num");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(255)
                .HasColumnName("ip_address");
            entity.Property(e => e.LoginDatetime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("login_datetime");
        });

        modelBuilder.Entity<UserInfo>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("user_info_pkey");

            entity.ToTable("user_info");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.AvatarThumbnail).HasColumnName("avatar_thumbnail");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
            entity.Property(e => e.Password)
                .HasMaxLength(200)
                .HasColumnName("password");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
