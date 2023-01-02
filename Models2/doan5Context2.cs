﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TinTucGameAPI.Models2
{
    public partial class doan5Context2 : DbContext
    {
        public doan5Context2()
        {
        }

        public doan5Context2(DbContextOptions<doan5Context2> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Feed> Feeds { get; set; } = null!;
        public virtual DbSet<Image> Images { get; set; } = null!;
        public virtual DbSet<MessNotification> MessNotifications { get; set; } = null!;
        public virtual DbSet<Message> Messages { get; set; } = null!;
        public virtual DbSet<Notification> Notifications { get; set; } = null!;
        public virtual DbSet<Post> Posts { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Room> Rooms { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<staff> staff { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DARKFLAMEMASTER\\SQLEXPRESS; Initial Catalog=doan5;Trusted_Connection=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .HasColumnName("id");

                entity.Property(e => e.Categoryid)
                    .HasMaxLength(50)
                    .HasColumnName("categoryid");

                entity.Property(e => e.Description)
                    .HasColumnType("ntext")
                    .HasColumnName("description");

                entity.Property(e => e.Slug)
                    .HasMaxLength(500)
                    .HasColumnName("slug");

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .HasColumnName("title");

                entity.HasOne(d => d.CategoryNavigation)
                    .WithMany(p => p.InverseCategoryNavigation)
                    .HasForeignKey(d => d.Categoryid)
                    .HasConstraintName("FK_Category_Category");
            });

            modelBuilder.Entity<Feed>(entity =>
            {
                entity.ToTable("Feed");

                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .HasColumnName("id");

                entity.Property(e => e.Content)
                    .HasColumnType("ntext")
                    .HasColumnName("content");

                entity.Property(e => e.PostId)
                    .HasMaxLength(50)
                    .HasColumnName("post_id");

                entity.Property(e => e.UserId)
                    .HasMaxLength(50)
                    .HasColumnName("user_id");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.Feeds)
                    .HasForeignKey(d => d.PostId)
                    .HasConstraintName("FK_Feed_Post");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Feeds)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Feed_User");
            });

            modelBuilder.Entity<Image>(entity =>
            {
                entity.ToTable("Image");

                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .HasColumnName("id");

                entity.Property(e => e.Image1)
                    .HasMaxLength(50)
                    .HasColumnName("image");
            });

            modelBuilder.Entity<MessNotification>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .HasColumnName("id");

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.MessId)
                    .HasMaxLength(50)
                    .HasColumnName("mess_id");

                entity.Property(e => e.OwnerId)
                    .HasMaxLength(50)
                    .HasColumnName("owner_id");

                entity.Property(e => e.Read).HasColumnName("read");

                entity.HasOne(d => d.Mess)
                    .WithMany(p => p.MessNotifications)
                    .HasForeignKey(d => d.MessId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_MessNotifications_Message");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.MessNotifications)
                    .HasForeignKey(d => d.OwnerId)
                    .HasConstraintName("FK_MessNotifications_Staff");
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("Message");

                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .HasColumnName("id");

                entity.Property(e => e.Content)
                    .HasColumnType("ntext")
                    .HasColumnName("content");

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.RoomId)
                    .HasMaxLength(50)
                    .HasColumnName("room_id");

                entity.Property(e => e.SenderId)
                    .HasMaxLength(50)
                    .HasColumnName("sender_id");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.Messages)
                    .HasForeignKey(d => d.RoomId)
                    .HasConstraintName("FK_Message_Room");

                entity.HasOne(d => d.Sender)
                    .WithMany(p => p.Messages)
                    .HasForeignKey(d => d.SenderId)
                    .HasConstraintName("FK_Message_Staff");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notification");

                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("date")
                    .HasColumnName("created_at");

                entity.Property(e => e.CreatorId)
                    .HasMaxLength(50)
                    .HasColumnName("creator_id");

                entity.Property(e => e.OwnerId)
                    .HasMaxLength(50)
                    .HasColumnName("owner_id");

                entity.Property(e => e.PostId)
                    .HasMaxLength(50)
                    .HasColumnName("post_id");

                entity.Property(e => e.Read)
                    .HasMaxLength(50)
                    .HasColumnName("read");

                entity.Property(e => e.Type)
                    .HasMaxLength(50)
                    .HasColumnName("type");
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.ToTable("Post");

                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .HasColumnName("id");

                entity.Property(e => e.Author)
                    .HasMaxLength(50)
                    .HasColumnName("author");

                entity.Property(e => e.Banner)
                    .HasColumnType("ntext")
                    .HasColumnName("banner");

                entity.Property(e => e.Content)
                    .HasColumnType("ntext")
                    .HasColumnName("content");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("date")
                    .HasColumnName("created_at");

                entity.Property(e => e.Description)
                    .HasColumnType("ntext")
                    .HasColumnName("description");

                entity.Property(e => e.Slug)
                    .HasMaxLength(500)
                    .HasColumnName("slug");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status");

                entity.Property(e => e.Title)
                    .HasMaxLength(250)
                    .HasColumnName("title");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("date")
                    .HasColumnName("updated_at");

                entity.Property(e => e.View).HasColumnName("view");

                entity.HasOne(d => d.AuthorNavigation)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.Author)
                    .HasConstraintName("FK_Post_User");

                entity.HasMany(d => d.Categories)
                    .WithMany(p => p.Posts)
                    .UsingEntity<Dictionary<string, object>>(
                        "PostCategory",
                        l => l.HasOne<Category>().WithMany().HasForeignKey("CategoryId").HasConstraintName("FK_PostCategory_Category"),
                        r => r.HasOne<Post>().WithMany().HasForeignKey("PostId").HasConstraintName("FK_PostCategory_Post"),
                        j =>
                        {
                            j.HasKey("PostId", "CategoryId");

                            j.ToTable("PostCategory");

                            j.IndexerProperty<string>("PostId").HasMaxLength(50).HasColumnName("post_id");

                            j.IndexerProperty<string>("CategoryId").HasMaxLength(50).HasColumnName("category_id");
                        });

                entity.HasMany(d => d.Images)
                    .WithMany(p => p.Posts)
                    .UsingEntity<Dictionary<string, object>>(
                        "PostImage",
                        l => l.HasOne<Image>().WithMany().HasForeignKey("ImageId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_PostImage_Image"),
                        r => r.HasOne<Post>().WithMany().HasForeignKey("PostId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_PostImage_Post"),
                        j =>
                        {
                            j.HasKey("PostId", "ImageId");

                            j.ToTable("PostImage");

                            j.IndexerProperty<string>("PostId").HasMaxLength(50).HasColumnName("post_id");

                            j.IndexerProperty<string>("ImageId").HasMaxLength(50).HasColumnName("image_id");
                        });
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .HasColumnName("id");

                entity.Property(e => e.Role1)
                    .HasMaxLength(50)
                    .HasColumnName("role");
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.ToTable("Room");

                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .HasColumnName("id");

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.Group).HasColumnName("group");

                entity.Property(e => e.LatestId)
                    .HasMaxLength(50)
                    .HasColumnName("latest_id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.HasOne(d => d.Latest)
                    .WithMany(p => p.Rooms)
                    .HasForeignKey(d => d.LatestId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Room_Message");

                entity.HasMany(d => d.Users)
                    .WithMany(p => p.Rooms)
                    .UsingEntity<Dictionary<string, object>>(
                        "Participant",
                        l => l.HasOne<staff>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Participants_Staff"),
                        r => r.HasOne<Room>().WithMany().HasForeignKey("RoomId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Participants_Room"),
                        j =>
                        {
                            j.HasKey("RoomId", "UserId");

                            j.ToTable("Participants");

                            j.IndexerProperty<string>("RoomId").HasMaxLength(50).HasColumnName("room_id");

                            j.IndexerProperty<string>("UserId").HasMaxLength(50).HasColumnName("user_id");
                        });
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .HasColumnName("id");

                entity.Property(e => e.Avatar).HasColumnName("avatar");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .HasColumnName("email");

                entity.Property(e => e.Passwordhash)
                    .HasMaxLength(50)
                    .HasColumnName("passwordhash");

                entity.Property(e => e.Salt)
                    .HasMaxLength(50)
                    .HasColumnName("salt");

                entity.HasMany(d => d.Roles)
                    .WithMany(p => p.Users)
                    .UsingEntity<Dictionary<string, object>>(
                        "UserRole",
                        l => l.HasOne<Role>().WithMany().HasForeignKey("RoleId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_UserRole_Role"),
                        r => r.HasOne<User>().WithMany().HasForeignKey("UserId").HasConstraintName("FK_UserRole_User"),
                        j =>
                        {
                            j.HasKey("UserId", "RoleId");

                            j.ToTable("UserRole");

                            j.IndexerProperty<string>("UserId").HasMaxLength(50).HasColumnName("user_id");

                            j.IndexerProperty<string>("RoleId").HasMaxLength(50).HasColumnName("role_id");
                        });
            });

            modelBuilder.Entity<staff>(entity =>
            {
                entity.ToTable("Staff");

                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .HasColumnName("id");

                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .HasColumnName("address");

                entity.Property(e => e.Birthdate)
                    .HasColumnType("date")
                    .HasColumnName("birthdate");

                entity.Property(e => e.Gender)
                    .HasMaxLength(3)
                    .HasColumnName("gender");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.Phone)
                    .HasMaxLength(50)
                    .HasColumnName("phone");

                entity.Property(e => e.UserId)
                    .HasMaxLength(50)
                    .HasColumnName("user_id");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}