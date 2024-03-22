using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIx.Migrations
{
    /// <inheritdoc />
    public partial class AddDateAndPostBackToConcilliation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostConcilliationUrl",
                table: "PaymentProvider");

            migrationBuilder.DropColumn(
                name: "OutputFileUrl",
                table: "Concilliation");

            migrationBuilder.AddColumn<DateOnly>(
                name: "Date",
                table: "Concilliation",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<string>(
                name: "Postback",
                table: "Concilliation",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "Concilliation");

            migrationBuilder.DropColumn(
                name: "Postback",
                table: "Concilliation");

            migrationBuilder.AddColumn<string>(
                name: "PostConcilliationUrl",
                table: "PaymentProvider",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OutputFileUrl",
                table: "Concilliation",
                type: "text",
                nullable: true);
        }
    }
}
