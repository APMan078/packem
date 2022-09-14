using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class Item_And_Vendor_NowHaveAManyToManyRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventories_CustomerLocations",
                table: "Inventories");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_CustomerLocations",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Vendors",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_CustomerLocations",
                table: "Vendors");

            migrationBuilder.DropIndex(
                name: "IX_Items_CustomerLocationId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "CustomerLocationId",
                table: "Items");

            migrationBuilder.RenameColumn(
                name: "CustomerLocationId",
                table: "Vendors",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Vendors_CustomerLocationId",
                table: "Vendors",
                newName: "IX_Vendors_CustomerId");

            migrationBuilder.RenameColumn(
                name: "VendorId",
                table: "Items",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Items_VendorId",
                table: "Items",
                newName: "IX_Items_CustomerId");

            migrationBuilder.RenameColumn(
                name: "CustomerLocationId",
                table: "Inventories",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Inventories_CustomerLocationId",
                table: "Inventories",
                newName: "IX_Inventories_CustomerId");

            migrationBuilder.CreateTable(
                name: "Items_Vendors",
                columns: table => new
                {
                    ItemVendorId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerId = table.Column<int>(type: "integer", nullable: true),
                    ItemId = table.Column<int>(type: "integer", nullable: true),
                    VendorId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items_Vendors", x => x.ItemVendorId);
                    table.ForeignKey(
                        name: "FK_ItemsVendors_Customers",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId");
                    table.ForeignKey(
                        name: "FK_ItemsVendors_Items",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId");
                    table.ForeignKey(
                        name: "FK_ItemsVendors_Vendors",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "VendorId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Items_Vendors_CustomerId",
                table: "Items_Vendors",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Vendors_ItemId",
                table: "Items_Vendors",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Vendors_VendorId",
                table: "Items_Vendors",
                column: "VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventories_Customers",
                table: "Inventories",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Customers",
                table: "Items",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendors_Customers",
                table: "Vendors",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventories_Customers",
                table: "Inventories");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Customers",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_Customers",
                table: "Vendors");

            migrationBuilder.DropTable(
                name: "Items_Vendors");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "Vendors",
                newName: "CustomerLocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Vendors_CustomerId",
                table: "Vendors",
                newName: "IX_Vendors_CustomerLocationId");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "Items",
                newName: "VendorId");

            migrationBuilder.RenameIndex(
                name: "IX_Items_CustomerId",
                table: "Items",
                newName: "IX_Items_VendorId");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "Inventories",
                newName: "CustomerLocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Inventories_CustomerId",
                table: "Inventories",
                newName: "IX_Inventories_CustomerLocationId");

            migrationBuilder.AddColumn<int>(
                name: "CustomerLocationId",
                table: "Items",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_CustomerLocationId",
                table: "Items",
                column: "CustomerLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventories_CustomerLocations",
                table: "Inventories",
                column: "CustomerLocationId",
                principalTable: "CustomerLocations",
                principalColumn: "CustomerLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_CustomerLocations",
                table: "Items",
                column: "CustomerLocationId",
                principalTable: "CustomerLocations",
                principalColumn: "CustomerLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Vendors",
                table: "Items",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendors_CustomerLocations",
                table: "Vendors",
                column: "CustomerLocationId",
                principalTable: "CustomerLocations",
                principalColumn: "CustomerLocationId");
        }
    }
}
