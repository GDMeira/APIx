using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIx.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentProviderIndexTokenAndUrls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PatchPaymentUrl",
                table: "PaymentProvider",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PostPaymentUrl",
                table: "PaymentProvider",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Payment",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Payment",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentProviderAccount_Token",
                table: "PaymentProvider",
                column: "Token",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PaymentProviderAccount_Token",
                table: "PaymentProvider");

            migrationBuilder.DropColumn(
                name: "PatchPaymentUrl",
                table: "PaymentProvider");

            migrationBuilder.DropColumn(
                name: "PostPaymentUrl",
                table: "PaymentProvider");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Payment",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "NOW()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Payment",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "NOW()");
        }
    }
}
