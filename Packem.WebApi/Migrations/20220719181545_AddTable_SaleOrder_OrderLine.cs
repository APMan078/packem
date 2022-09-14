using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class AddTable_SaleOrder_OrderLine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SaleOrders",
                columns: table => new
                {
                    SaleOrderId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerLocationId = table.Column<int>(type: "integer", nullable: true),
                    CustomerFacilityId = table.Column<int>(type: "integer", nullable: true),
                    SaleOrderNo = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PromiseDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    OrderQty = table.Column<int>(type: "integer", nullable: false),
                    CustomerNo = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ShipToCity = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ShipToAddress1 = table.Column<string>(type: "character varying(550)", maxLength: 550, nullable: false),
                    ShipToAddress2 = table.Column<string>(type: "character varying(550)", maxLength: 550, nullable: true),
                    ShipToPhoneNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleOrders", x => x.SaleOrderId);
                    table.ForeignKey(
                        name: "FK_SaleOrders_CustomerFacilities",
                        column: x => x.CustomerFacilityId,
                        principalTable: "CustomerFacilities",
                        principalColumn: "CustomerFacilityId");
                    table.ForeignKey(
                        name: "FK_SaleOrders_CustomerLocations",
                        column: x => x.CustomerLocationId,
                        principalTable: "CustomerLocations",
                        principalColumn: "CustomerLocationId");
                });

            migrationBuilder.CreateTable(
                name: "OrderLines",
                columns: table => new
                {
                    OrderLineId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerLocationId = table.Column<int>(type: "integer", nullable: true),
                    SaleOrderId = table.Column<int>(type: "integer", nullable: true),
                    ItemId = table.Column<int>(type: "integer", nullable: true),
                    Qty = table.Column<int>(type: "integer", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderLines", x => x.OrderLineId);
                    table.ForeignKey(
                        name: "FK_OrderLines_CustomerLocations",
                        column: x => x.CustomerLocationId,
                        principalTable: "CustomerLocations",
                        principalColumn: "CustomerLocationId");
                    table.ForeignKey(
                        name: "FK_OrderLines_Items",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId");
                    table.ForeignKey(
                        name: "FK_OrderLines_SaleOrders",
                        column: x => x.SaleOrderId,
                        principalTable: "SaleOrders",
                        principalColumn: "SaleOrderId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderLines_CustomerLocationId",
                table: "OrderLines",
                column: "CustomerLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderLines_Deleted",
                table: "OrderLines",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_OrderLines_ItemId",
                table: "OrderLines",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderLines_SaleOrderId",
                table: "OrderLines",
                column: "SaleOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrders_CustomerFacilityId",
                table: "SaleOrders",
                column: "CustomerFacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrders_CustomerLocationId",
                table: "SaleOrders",
                column: "CustomerLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrders_Deleted",
                table: "SaleOrders",
                column: "Deleted");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderLines");

            migrationBuilder.DropTable(
                name: "SaleOrders");
        }
    }
}
