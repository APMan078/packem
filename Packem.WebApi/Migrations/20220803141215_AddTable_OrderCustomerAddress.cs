using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class AddTable_OrderCustomerAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BillingAddressId",
                table: "SaleOrders",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShippingAddressId",
                table: "SaleOrders",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrderCustomerAddresses",
                columns: table => new
                {
                    OrderCustomerAddressId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AddressType = table.Column<int>(type: "integer", nullable: false),
                    OrderCustomerId = table.Column<int>(type: "integer", nullable: true),
                    Address1 = table.Column<string>(type: "character varying(550)", maxLength: 550, nullable: false),
                    Address2 = table.Column<string>(type: "character varying(550)", maxLength: 550, nullable: true),
                    StateProvince = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ZipPostalCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Country = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderCustomerAddresses", x => x.OrderCustomerAddressId);
                    table.ForeignKey(
                        name: "FK_OrderCustomerAddresses_OrderCustomers",
                        column: x => x.OrderCustomerId,
                        principalTable: "OrderCustomers",
                        principalColumn: "OrderCustomerId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrders_BillingAddressId",
                table: "SaleOrders",
                column: "BillingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrders_ShippingAddressId",
                table: "SaleOrders",
                column: "ShippingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderCustomerAddresses_Deleted",
                table: "OrderCustomerAddresses",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_OrderCustomerAddresses_OrderCustomerId",
                table: "OrderCustomerAddresses",
                column: "OrderCustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrders_BillingAddresses",
                table: "SaleOrders",
                column: "BillingAddressId",
                principalTable: "OrderCustomerAddresses",
                principalColumn: "OrderCustomerAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrders_ShippingAddresses",
                table: "SaleOrders",
                column: "ShippingAddressId",
                principalTable: "OrderCustomerAddresses",
                principalColumn: "OrderCustomerAddressId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrders_BillingAddresses",
                table: "SaleOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrders_ShippingAddresses",
                table: "SaleOrders");

            migrationBuilder.DropTable(
                name: "OrderCustomerAddresses");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrders_BillingAddressId",
                table: "SaleOrders");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrders_ShippingAddressId",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "BillingAddressId",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "ShippingAddressId",
                table: "SaleOrders");
        }
    }
}
