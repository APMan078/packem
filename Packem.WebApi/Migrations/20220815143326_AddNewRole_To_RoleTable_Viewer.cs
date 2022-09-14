using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class AddNewRole_To_RoleTable_Viewer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"INSERT INTO ""Roles"" (""RoleId"",""Name"", ""Deleted"") OVERRIDING SYSTEM VALUE VALUES (5, 'Viewer', false);");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DELETE FROM ""Roles"" WHERE ""RoleId"" = 5;");
        }
    }
}