using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace APIx.Migrations
{
    /// <inheritdoc />
    public partial class CreateDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentProvider",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    Token = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentProvider", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    Cpf = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.UniqueConstraint("AK_User_Cpf", x => x.Cpf);
                });

            migrationBuilder.CreateTable(
                name: "PaymentProviderAccount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    Number = table.Column<string>(type: "text", nullable: false),
                    Agency = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    PaymentProviderId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentProviderAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentProviderAccount_PaymentProvider_PaymentProviderId",
                        column: x => x.PaymentProviderId,
                        principalTable: "PaymentProvider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentProviderAccount_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PixKey",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    PaymentProviderAccountId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PixKey", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PixKey_PaymentProviderAccount_PaymentProviderAccountId",
                        column: x => x.PaymentProviderAccountId,
                        principalTable: "PaymentProviderAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.UniqueConstraint("AK_PixKey_Value", x => x.Value);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentProviderAccount_PaymentProviderId",
                table: "PaymentProviderAccount",
                column: "PaymentProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentProviderAccount_UserId",
                table: "PaymentProviderAccount",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PixKey_PaymentProviderAccountId",
                table: "PixKey",
                column: "PaymentProviderAccountId");

            migrationBuilder.InsertData(
            table: "User",
            columns: ["Cpf", "Name"],
            values: ["11111111111", "User1"]);

            migrationBuilder.InsertData(
            table: "User",
            columns: ["Cpf", "Name"],
            values: ["22222222222", "User2"]);

            migrationBuilder.InsertData(
            table: "User",
            columns: ["Cpf", "Name"],
            values: ["33333333333", "User3"]);

            migrationBuilder.InsertData(
            table: "PaymentProvider",
            columns: ["Name", "Token"],
            values: ["Stone", "07lcA2MyZGHKYAEYhoG1IVKZ0ZxDEOxvrZETaqmGoOsxUX7jM0lDFlakmEKb8tYc"]);

            migrationBuilder.InsertData(
            table: "PaymentProvider",
            columns: ["Name", "Token"],
            values: ["Nu Bank", "V6lT3I1JPkjxVhheWjqFeuycU09i7Xj2zptwXPMo7pDifAPgGb29BgHam2Vo3Hd3"]);

            migrationBuilder.InsertData(
            table: "PaymentProviderAccount",
            columns: ["Number", "Agency", "UserId", "PaymentProviderId"],
            values: ["169315", "42528", 1, 1]);

            migrationBuilder.InsertData(
            table: "PaymentProviderAccount",
            columns: ["Number", "Agency", "UserId", "PaymentProviderId"],
            values: ["169316", "42529", 1, 2]);

            migrationBuilder.InsertData(
            table: "PaymentProviderAccount",
            columns: ["Number", "Agency", "UserId", "PaymentProviderId"],
            values: ["123456", "12345", 2, 1]);

            migrationBuilder.InsertData(
            table: "PaymentProviderAccount",
            columns: ["Number", "Agency", "UserId", "PaymentProviderId"],
            values: ["654321", "54321", 2, 1]);

            migrationBuilder.InsertData(
            table: "PixKey",
            columns: ["Type", "Value", "PaymentProviderAccountId"],
            values: ["CPF", "11111111111", 1]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PixKey");

            migrationBuilder.DropTable(
                name: "PaymentProviderAccount");

            migrationBuilder.DropTable(
                name: "PaymentProvider");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
