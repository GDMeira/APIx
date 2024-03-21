using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace APIx.Migrations
{
    /// <inheritdoc />
    public partial class AddConcilliation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Concilliation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FileUrl = table.Column<string>(type: "text", nullable: false),
                    PaymentProviderId = table.Column<int>(type: "integer", nullable: false),
                    PaymentProviderAccountId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Concilliation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Concilliation_PaymentProviderAccount_PaymentProviderAccount~",
                        column: x => x.PaymentProviderAccountId,
                        principalTable: "PaymentProviderAccount",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Concilliation_PaymentProvider_PaymentProviderId",
                        column: x => x.PaymentProviderId,
                        principalTable: "PaymentProvider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Concilliation_PaymentProviderAccountId",
                table: "Concilliation",
                column: "PaymentProviderAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Concilliation_PaymentProviderId",
                table: "Concilliation",
                column: "PaymentProviderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Concilliation");
        }
    }
}
