using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TinTucGameAPI.Models
{
    public partial class doan5Context : DbContext
    {
        public doan5Context()
        {
        }

        public doan5Context(DbContextOptions<doan5Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Image> Images { get; set; } = null!;
        public virtual DbSet<Post> Posts { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<staff> staff { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=Hieu\\SQLEXPRESS; Initial Catalog=doan5;Trusted_Connection=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.HasIndex(e => e.Categoryid, "IX_Category_categoryid");

                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .HasColumnName("id");

                entity.Property(e => e.Categoryid)
                    .HasMaxLength(50)
                    .HasColumnName("categoryid");

                entity.Property(e => e.Description)
                    .HasColumnType("ntext")
                    .HasColumnName("description");

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .HasColumnName("title");

                entity.HasOne(d => d.CategoryNavigation)
                    .WithMany(p => p.InverseCategoryNavigation)
                    .HasForeignKey(d => d.Categoryid)
                    .HasConstraintName("FK_Category_Category");
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

            modelBuilder.Entity<Post>(entity =>
            {
                entity.ToTable("Post");

                entity.HasIndex(e => e.Author, "IX_Post_author");

                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .HasColumnName("id");

                entity.Property(e => e.Author)
                    .HasMaxLength(50)
                    .HasColumnName("author");

                entity.Property(e => e.Content)
                    .HasColumnType("ntext")
                    .HasColumnName("content");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("date")
                    .HasColumnName("created_at");

                entity.Property(e => e.Description)
                    .HasColumnType("ntext")
                    .HasColumnName("description");

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .HasColumnName("title");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("date")
                    .HasColumnName("updated_at");

                entity.HasOne(d => d.AuthorNavigation)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.Author)
                    .HasConstraintName("FK_Post_User");

                entity.HasMany(d => d.Categories)
                    .WithMany(p => p.Posts)
                    .UsingEntity<Dictionary<string, object>>(
                        "PostCategory",
                        l => l.HasOne<Category>().WithMany().HasForeignKey("CategoryId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_PostCategory_Category"),
                        r => r.HasOne<Post>().WithMany().HasForeignKey("PostId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_PostCategory_Post"),
                        j =>
                        {
                            j.HasKey("PostId", "CategoryId");

                            j.ToTable("PostCategory");

                            j.HasIndex(new[] { "CategoryId" }, "IX_PostCategory_category_id");

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

                            j.HasIndex(new[] { "ImageId" }, "IX_PostImage_image_id");

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
                        r => r.HasOne<User>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_UserRole_User"),
                        j =>
                        {
                            j.HasKey("UserId", "RoleId");

                            j.ToTable("UserRole");

                            j.HasIndex(new[] { "RoleId" }, "IX_UserRole_role_id");

                            j.IndexerProperty<string>("UserId").HasMaxLength(50).HasColumnName("user_id");

                            j.IndexerProperty<string>("RoleId").HasMaxLength(50).HasColumnName("role_id");
                        });
               
            });

            modelBuilder.Entity<staff>(entity =>
            {
                entity.ToTable("Staff");

                entity.HasIndex(e => e.UserId, "IX_Staff_user_id");

                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .HasColumnName("id");

                entity.Property(e => e.Address)
                    .HasMaxLength(50)
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

                entity.HasOne(d => d.User)
                    .WithMany(p => p.staff)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Staff_User");
            });

           

            //modelBuilder.Entity<UserRole>()
            //    .HasOne(u => u.User)
            //    .WithMany(u => u.UserRoles)
            //    .HasForeignKey(u => u.User_id);
            //modelBuilder.Entity<UserRole>()
            //   .HasOne(u => u.Role)
            //   .WithMany(u => u.UserRoles)
            //   .HasForeignKey(u => u.Role_id);

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
