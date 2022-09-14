using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class AddCustomerDevice_AddCustomerDeviceToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerDevices",
                columns: table => new
                {
                    CustomerDeviceId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerLocationId = table.Column<int>(type: "integer", nullable: true),
                    SerialNumber = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    AddedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DeactivedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastLoginDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerDevices", x => x.CustomerDeviceId);
                    table.ForeignKey(
                        name: "FK_CustomerDevices_CustomerLocations",
                        column: x => x.CustomerLocationId,
                        principalTable: "CustomerLocations",
                        principalColumn: "CustomerLocationId");
                });

            migrationBuilder.CreateTable(
                name: "CustomerDeviceTokens",
                columns: table => new
                {
                    CustomerDeviceTokenId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerDeviceId = table.Column<int>(type: "integer", nullable: true),
                    DeviceToken = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    AddedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsValidated = table.Column<bool>(type: "boolean", nullable: false),
                    ValidatedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DeactivedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerDeviceTokens", x => x.CustomerDeviceTokenId);
                    table.ForeignKey(
                        name: "FK_CustomerDeviceTokens_CustomerDevices",
                        column: x => x.CustomerDeviceId,
                        principalTable: "CustomerDevices",
                        principalColumn: "CustomerDeviceId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerDevices_CustomerLocationId",
                table: "CustomerDevices",
                column: "CustomerLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerDeviceTokens_CustomerDeviceId",
                table: "CustomerDeviceTokens",
                column: "CustomerDeviceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerDeviceTokens");

            migrationBuilder.DropTable(
                name: "CustomerDevices");
        }
    }
}
