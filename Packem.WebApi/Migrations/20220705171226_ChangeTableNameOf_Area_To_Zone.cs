using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class ChangeTableNameOf_Area_To_Zone : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bins_Areas",
                table: "Bins");

            migrationBuilder.DropTable(
                name: "Inventories_Areas");

            migrationBuilder.DropTable(
                name: "Areas");

            migrationBuilder.RenameColumn(
                name: "AreaId",
                table: "Bins",
                newName: "ZoneId");

            migrationBuilder.RenameIndex(
                name: "IX_Bins_AreaId",
                table: "Bins",
                newName: "IX_Bins_ZoneId");

            migrationBuilder.CreateTable(
                name: "Zones",
                columns: table => new
                {
                    ZoneId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerLocationId = table.Column<int>(type: "integer", nullable: true),
                    CustomerFacilityId = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zones", x => x.ZoneId);
                    table.ForeignKey(
                        name: "FK_Zones_CustomerFacilities",
                        column: x => x.CustomerFacilityId,
                        principalTable: "CustomerFacilities",
                        principalColumn: "CustomerFacilityId");
                    table.ForeignKey(
                        name: "FK_Zones_CustomerLocations",
                        column: x => x.CustomerLocationId,
                        principalTable: "CustomerLocations",
                        principalColumn: "CustomerLocationId");
                });

            migrationBuilder.CreateTable(
                name: "Inventories_Zones",
                columns: table => new
                {
                    InventoryZoneId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerLocationId = table.Column<int>(type: "integer", nullable: true),
                    InventoryId = table.Column<int>(type: "integer", nullable: true),
                    ZoneId = table.Column<int>(type: "integer", nullable: true),
                    Qty = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventories_Zones", x => x.InventoryZoneId);
                    table.ForeignKey(
                        name: "FK_InventoryZones_Areas",
                        column: x => x.ZoneId,
                        principalTable: "Zones",
                        principalColumn: "ZoneId");
                    table.ForeignKey(
                        name: "FK_InventoryZones_CustomerLocations",
                        column: x => x.CustomerLocationId,
                        principalTable: "CustomerLocations",
                        principalColumn: "CustomerLocationId");
                    table.ForeignKey(
                        name: "FK_InventoryZones_Inventories",
                        column: x => x.InventoryId,
                        principalTable: "Inventories",
                        principalColumn: "InventoryId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_Zones_CustomerLocationId",
                table: "Inventories_Zones",
                column: "CustomerLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_Zones_InventoryId",
                table: "Inventories_Zones",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_Zones_ZoneId",
                table: "Inventories_Zones",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_Zones_CustomerFacilityId",
                table: "Zones",
                column: "CustomerFacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Zones_CustomerLocationId",
                table: "Zones",
                column: "CustomerLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bins_Zones",
                table: "Bins",
                column: "ZoneId",
                principalTable: "Zones",
                principalColumn: "ZoneId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bins_Zones",
                table: "Bins");

            migrationBuilder.DropTable(
                name: "Inventories_Zones");

            migrationBuilder.DropTable(
                name: "Zones");

            migrationBuilder.RenameColumn(
                name: "ZoneId",
                table: "Bins",
                newName: "AreaId");

            migrationBuilder.RenameIndex(
                name: "IX_Bins_ZoneId",
                table: "Bins",
                newName: "IX_Bins_AreaId");

            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table => new
                {
                    AreaId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerFacilityId = table.Column<int>(type: "integer", nullable: true),
                    CustomerLocationId = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.AreaId);
                    table.ForeignKey(
                        name: "FK_Areas_CustomerFacilities",
                        column: x => x.CustomerFacilityId,
                        principalTable: "CustomerFacilities",
                        principalColumn: "CustomerFacilityId");
                    table.ForeignKey(
                        name: "FK_Areas_CustomerLocations",
                        column: x => x.CustomerLocationId,
                        principalTable: "CustomerLocations",
                        principalColumn: "CustomerLocationId");
                });

            migrationBuilder.CreateTable(
                name: "Inventories_Areas",
                columns: table => new
                {
                    InventoryAreaId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AreaId = table.Column<int>(type: "integer", nullable: true),
                    CustomerLocationId = table.Column<int>(type: "integer", nullable: true),
                    InventoryId = table.Column<int>(type: "integer", nullable: true),
                    Qty = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventories_Areas", x => x.InventoryAreaId);
                    table.ForeignKey(
                        name: "FK_InventoriesAreas_Areas",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "AreaId");
                    table.ForeignKey(
                        name: "FK_InventoriesAreas_CustomerLocations",
                        column: x => x.CustomerLocationId,
                        principalTable: "CustomerLocations",
                        principalColumn: "CustomerLocationId");
                    table.ForeignKey(
                        name: "FK_InventoriesAreas_Inventories",
                        column: x => x.InventoryId,
                        principalTable: "Inventories",
                        principalColumn: "InventoryId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Areas_CustomerFacilityId",
                table: "Areas",
                column: "CustomerFacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Areas_CustomerLocationId",
                table: "Areas",
                column: "CustomerLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_Areas_AreaId",
                table: "Inventories_Areas",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_Areas_CustomerLocationId",
                table: "Inventories_Areas",
                column: "CustomerLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_Areas_InventoryId",
                table: "Inventories_Areas",
                column: "InventoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bins_Areas",
                table: "Bins",
                column: "AreaId",
                principalTable: "Areas",
                principalColumn: "AreaId");
        }
    }
}
