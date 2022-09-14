using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class RenameTable_Location_To_Area : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Locations_Locations",
                table: "Bins");

            migrationBuilder.DropTable(
                name: "Inventories_Locations");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropColumn(
                name: "UseLocation",
                table: "CustomerFacilities");

            migrationBuilder.RenameColumn(
                name: "LocationId",
                table: "Bins",
                newName: "AreaId");

            migrationBuilder.RenameIndex(
                name: "IX_Bins_LocationId",
                table: "Bins",
                newName: "IX_Bins_AreaId");

            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table => new
                {
                    AreaId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerLocationId = table.Column<int>(type: "integer", nullable: true),
                    CustomerFacilityId = table.Column<int>(type: "integer", nullable: true),
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
                    CustomerLocationId = table.Column<int>(type: "integer", nullable: true),
                    InventoryId = table.Column<int>(type: "integer", nullable: true),
                    AreaId = table.Column<int>(type: "integer", nullable: true),
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

        protected override void Down(MigrationBuilder migrationBuilder)
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
                newName: "LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Bins_AreaId",
                table: "Bins",
                newName: "IX_Bins_LocationId");

            migrationBuilder.AddColumn<bool>(
                name: "UseLocation",
                table: "CustomerFacilities",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    LocationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerFacilityId = table.Column<int>(type: "integer", nullable: true),
                    CustomerLocationId = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.LocationId);
                    table.ForeignKey(
                        name: "FK_Locations_CustomerFacilities",
                        column: x => x.CustomerFacilityId,
                        principalTable: "CustomerFacilities",
                        principalColumn: "CustomerFacilityId");
                    table.ForeignKey(
                        name: "FK_Locations_CustomerLocations",
                        column: x => x.CustomerLocationId,
                        principalTable: "CustomerLocations",
                        principalColumn: "CustomerLocationId");
                });

            migrationBuilder.CreateTable(
                name: "Inventories_Locations",
                columns: table => new
                {
                    InventoryLocationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerLocationId = table.Column<int>(type: "integer", nullable: true),
                    InventoryId = table.Column<int>(type: "integer", nullable: true),
                    LocationId = table.Column<int>(type: "integer", nullable: true),
                    Qty = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventories_Locations", x => x.InventoryLocationId);
                    table.ForeignKey(
                        name: "FK_InventoriesLocations_CustomerLocations",
                        column: x => x.CustomerLocationId,
                        principalTable: "CustomerLocations",
                        principalColumn: "CustomerLocationId");
                    table.ForeignKey(
                        name: "FK_InventoriesLocations_Inventories",
                        column: x => x.InventoryId,
                        principalTable: "Inventories",
                        principalColumn: "InventoryId");
                    table.ForeignKey(
                        name: "FK_InventoriesLocations_Locations",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "LocationId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_Locations_CustomerLocationId",
                table: "Inventories_Locations",
                column: "CustomerLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_Locations_InventoryId",
                table: "Inventories_Locations",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_Locations_LocationId",
                table: "Inventories_Locations",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_CustomerFacilityId",
                table: "Locations",
                column: "CustomerFacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_CustomerLocationId",
                table: "Locations",
                column: "CustomerLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_Locations",
                table: "Bins",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "LocationId");
        }
    }
}
