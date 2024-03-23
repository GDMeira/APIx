using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIx.Migrations
{
    /// <inheritdoc />
    public partial class AddOutputFileUrlInConcilliation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OutputFileUrl",
                table: "Concilliation",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OutputFileUrl",
                table: "Concilliation");
        }
    }
}
