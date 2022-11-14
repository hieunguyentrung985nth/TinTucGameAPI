using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TinTucGameAPI.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "ntext", nullable: true),
                    categoryid = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.id);
                    table.ForeignKey(
                        name: "FK_Category_Category",
                        column: x => x.categoryid,
                        principalTable: "Category",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Image",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    image = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Image", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    passwordhash = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    salt = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    avatar = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Post",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "ntext", nullable: true),
                    content = table.Column<string>(type: "ntext", nullable: true),
                    author = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "date", nullable: true),
                    updated_at = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post", x => x.id);
                    table.ForeignKey(
                        name: "FK_Post_User",
                        column: x => x.author,
                        principalTable: "User",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Staff",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    gender = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    address = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    birthdate = table.Column<DateTime>(type: "date", nullable: true),
                    phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    user_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staff", x => x.id);
                    table.ForeignKey(
                        name: "FK_Staff_User",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    role_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "FK_UserRole_Role",
                        column: x => x.role_id,
                        principalTable: "Role",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_UserRole_User",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "PostCategory",
                columns: table => new
                {
                    post_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    category_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostCategory", x => new { x.post_id, x.category_id });
                    table.ForeignKey(
                        name: "FK_PostCategory_Category",
                        column: x => x.category_id,
                        principalTable: "Category",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_PostCategory_Post",
                        column: x => x.post_id,
                        principalTable: "Post",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "PostImage",
                columns: table => new
                {
                    post_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    image_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostImage", x => new { x.post_id, x.image_id });
                    table.ForeignKey(
                        name: "FK_PostImage_Image",
                        column: x => x.image_id,
                        principalTable: "Image",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_PostImage_Post",
                        column: x => x.post_id,
                        principalTable: "Post",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Category_categoryid",
                table: "Category",
                column: "categoryid");

            migrationBuilder.CreateIndex(
                name: "IX_Post_author",
                table: "Post",
                column: "author");

            migrationBuilder.CreateIndex(
                name: "IX_PostCategory_category_id",
                table: "PostCategory",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_PostImage_image_id",
                table: "PostImage",
                column: "image_id");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_user_id",
                table: "Staff",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_role_id",
                table: "UserRole",
                column: "role_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostCategory");

            migrationBuilder.DropTable(
                name: "PostImage");

            migrationBuilder.DropTable(
                name: "Staff");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Image");

            migrationBuilder.DropTable(
                name: "Post");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
