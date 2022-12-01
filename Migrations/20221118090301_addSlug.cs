using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TinTucGameAPI.Migrations
{
    public partial class addSlug : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Post",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Category",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Category");

        }
    }
}
