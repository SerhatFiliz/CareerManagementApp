using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareerManagementApp.WEB.Migrations
{
    /// <inheritdoc />
    public partial class aiChat_update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AIResponse",
                table: "AIChats",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AIResponse",
                table: "AIChats");
        }
    }
}
