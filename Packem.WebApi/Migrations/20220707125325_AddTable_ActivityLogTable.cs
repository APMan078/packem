using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class AddTable_ActivityLogTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Vendors",
                newName: "Address1");

            migrationBuilder.AddColumn<string>(
                name: "Address2",
                table: "Vendors",
                type: "character varying(550)",
                maxLength: 550,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Vendors",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StateProvince",
                table: "Vendors",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ZipPostalCode",
                table: "Vendors",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ActivityLogs",
                columns: table => new
                {
                    ActivityLogId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerId = table.Column<int>(type: "integer", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    InventoryId = table.Column<int>(type: "integer", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    ActivityDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Qty = table.Column<int>(type: "integer", nullable: false),
                    ZoneId = table.Column<int>(type: "integer", nullable: true),
                    BinId = table.Column<int>(type: "integer", nullable: true),
                    Notes = table.Column<string>(type: "character varying(550)", maxLength: 550, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLogs", x => x.ActivityLogId);
                    table.ForeignKey(
                        name: "FK_ActivityLogs_Bins",
                        column: x => x.BinId,
                        principalTable: "Bins",
                        principalColumn: "BinId");
                    table.ForeignKey(
                        name: "FK_ActivityLogs_Customers",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId");
                    table.ForeignKey(
                        name: "FK_ActivityLogs_Inventories",
                        column: x => x.InventoryId,
                        principalTable: "Inventories",
                        principalColumn: "InventoryId");
                    table.ForeignKey(
                        name: "FK_ActivityLogs_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_ActivityLogs_Zones",
                        column: x => x.ZoneId,
                        principalTable: "Zones",
                        principalColumn: "ZoneId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_BinId",
                table: "ActivityLogs",
                column: "BinId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_CustomerId",
                table: "ActivityLogs",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_InventoryId",
                table: "ActivityLogs",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_UserId",
                table: "ActivityLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_ZoneId",
                table: "ActivityLogs",
                column: "ZoneId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityLogs");

            migrationBuilder.DropColumn(
                name: "Address2",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "StateProvince",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "ZipPostalCode",
                table: "Vendors");

            migrationBuilder.RenameColumn(
                name: "Address1",
                table: "Vendors",
                newName: "Address");
        }
    }
}
