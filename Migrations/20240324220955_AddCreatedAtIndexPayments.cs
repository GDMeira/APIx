using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIx.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedAtIndexPayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Payment_CreatedAt",
                table: "Payment",
                column: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payment_CreatedAt",
                table: "Payment");
        }
    }
}
