using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DTE_2802_1_25H_Oblig1_MVC.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnerIdToModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Posts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Comments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Blogs",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Blogs");
        }
    }
}
