using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class CreateTable_OrderCustomer_OrderCustomerShippingAddress_OrderCustomerBillingAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderCustomerBillingAddressId",
                table: "SaleOrders",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderCustomerId",
                table: "SaleOrders",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderCustomerShippingAddressId",
                table: "SaleOrders",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrderCustomers",
                columns: table => new
                {
                    OrderCustomerId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerId = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    PaymentType = table.Column<int>(type: "integer", nullable: false),
                    LastOrderDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderCustomers", x => x.OrderCustomerId);
                    table.ForeignKey(
                        name: "FK_OrderCustomers_Customers",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId");
                });

            migrationBuilder.CreateTable(
                name: "OrderCustomerBillingAddresses",
                columns: table => new
                {
                    OrderCustomerBillingAddressId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderCustomerId = table.Column<int>(type: "integer", nullable: true),
                    Address1 = table.Column<string>(type: "character varying(550)", maxLength: 550, nullable: false),
                    Address2 = table.Column<string>(type: "character varying(550)", maxLength: 550, nullable: true),
                    StateProvince = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ZipPostalCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Country = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderCustomerBillingAddresses", x => x.OrderCustomerBillingAddressId);
                    table.ForeignKey(
                        name: "FK_OrderCustomerBillingAddresses_OrderCustomers",
                        column: x => x.OrderCustomerId,
                        principalTable: "OrderCustomers",
                        principalColumn: "OrderCustomerId");
                });

            migrationBuilder.CreateTable(
                name: "OrderCustomerShippingAddresses",
                columns: table => new
                {
                    OrderCustomerShippingAddressId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderCustomerId = table.Column<int>(type: "integer", nullable: true),
                    Address1 = table.Column<string>(type: "character varying(550)", maxLength: 550, nullable: false),
                    Address2 = table.Column<string>(type: "character varying(550)", maxLength: 550, nullable: true),
                    StateProvince = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ZipPostalCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Country = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderCustomerShippingAddresses", x => x.OrderCustomerShippingAddressId);
                    table.ForeignKey(
                        name: "FK_OrderCustomerShippingAddresses_OrderCustomers",
                        column: x => x.OrderCustomerId,
                        principalTable: "OrderCustomers",
                        principalColumn: "OrderCustomerId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrders_OrderCustomerBillingAddressId",
                table: "SaleOrders",
                column: "OrderCustomerBillingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrders_OrderCustomerId",
                table: "SaleOrders",
                column: "OrderCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrders_OrderCustomerShippingAddressId",
                table: "SaleOrders",
                column: "OrderCustomerShippingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderCustomerBillingAddresses_Deleted",
                table: "OrderCustomerBillingAddresses",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_OrderCustomerBillingAddresses_OrderCustomerId",
                table: "OrderCustomerBillingAddresses",
                column: "OrderCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderCustomers_CustomerId",
                table: "OrderCustomers",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderCustomers_Deleted",
                table: "OrderCustomers",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_OrderCustomerShippingAddresses_Deleted",
                table: "OrderCustomerShippingAddresses",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_OrderCustomerShippingAddresses_OrderCustomerId",
                table: "OrderCustomerShippingAddresses",
                column: "OrderCustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrders_OrderCustomerBillingAddresses",
                table: "SaleOrders",
                column: "OrderCustomerBillingAddressId",
                principalTable: "OrderCustomerBillingAddresses",
                principalColumn: "OrderCustomerBillingAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrders_OrderCustomers",
                table: "SaleOrders",
                column: "OrderCustomerId",
                principalTable: "OrderCustomers",
                principalColumn: "OrderCustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrders_OrderCustomerShippingAddresses",
                table: "SaleOrders",
                column: "OrderCustomerShippingAddressId",
                principalTable: "OrderCustomerShippingAddresses",
                principalColumn: "OrderCustomerShippingAddressId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrders_OrderCustomerBillingAddresses",
                table: "SaleOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrders_OrderCustomers",
                table: "SaleOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrders_OrderCustomerShippingAddresses",
                table: "SaleOrders");

            migrationBuilder.DropTable(
                name: "OrderCustomerBillingAddresses");

            migrationBuilder.DropTable(
                name: "OrderCustomerShippingAddresses");

            migrationBuilder.DropTable(
                name: "OrderCustomers");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrders_OrderCustomerBillingAddressId",
                table: "SaleOrders");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrders_OrderCustomerId",
                table: "SaleOrders");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrders_OrderCustomerShippingAddressId",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "OrderCustomerBillingAddressId",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "OrderCustomerId",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "OrderCustomerShippingAddressId",
                table: "SaleOrders");
        }
    }
}
