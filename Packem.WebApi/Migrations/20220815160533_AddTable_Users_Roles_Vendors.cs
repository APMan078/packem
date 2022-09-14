using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class AddTable_Users_Roles_Vendors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users_Roles_Vendors",
                columns: table => new
                {
                    UserRoleVendorId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    RoleId = table.Column<int>(type: "integer", nullable: true),
                    VendorId = table.Column<int>(type: "integer", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users_Roles_Vendors", x => x.UserRoleVendorId);
                    table.ForeignKey(
                        name: "FK_UsersRolesVendors_Roles",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId");
                    table.ForeignKey(
                        name: "FK_UsersRolesVendors_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_UsersRolesVendors_Vendors",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "VendorId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Roles_Vendors_Deleted",
                table: "Users_Roles_Vendors",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Roles_Vendors_RoleId",
                table: "Users_Roles_Vendors",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Roles_Vendors_UserId",
                table: "Users_Roles_Vendors",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Roles_Vendors_VendorId",
                table: "Users_Roles_Vendors",
                column: "VendorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users_Roles_Vendors");
        }
    }
}
