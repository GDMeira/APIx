using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIx.Migrations
{
    /// <inheritdoc />
    public partial class AddUserCpfIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "AK_User_Cpf_Index",
                table: "User",
                column: "Cpf",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "AK_User_Cpf_Index",
                table: "User");
        }
    }
}
