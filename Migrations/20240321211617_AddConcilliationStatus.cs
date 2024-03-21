using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIx.Migrations
{
    /// <inheritdoc />
    public partial class AddConcilliationStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Concilliation",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Concilliation");
        }
    }
}
