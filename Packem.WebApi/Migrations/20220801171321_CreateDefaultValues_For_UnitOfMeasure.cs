using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class CreateDefaultValues_For_UnitOfMeasure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"INSERT INTO ""UnitOfMeasures"" (""Code"", ""Description"", ""Type"", ""Deleted"") VALUES('BAG', 'Bag', 1, false);");
            migrationBuilder.Sql(@"INSERT INTO ""UnitOfMeasures"" (""Code"", ""Description"", ""Type"", ""Deleted"") VALUES('BKT', 'Bucket', 1, false);");
            migrationBuilder.Sql(@"INSERT INTO ""UnitOfMeasures"" (""Code"", ""Description"", ""Type"", ""Deleted"") VALUES('BND', 'Bundle', 1, false);");
            migrationBuilder.Sql(@"INSERT INTO ""UnitOfMeasures"" (""Code"", ""Description"", ""Type"", ""Deleted"") VALUES('BOWL', 'Bowl', 1, false);");
            migrationBuilder.Sql(@"INSERT INTO ""UnitOfMeasures"" (""Code"", ""Description"", ""Type"", ""Deleted"") VALUES('BX', 'Box', 1, false);");
            migrationBuilder.Sql(@"INSERT INTO ""UnitOfMeasures"" (""Code"", ""Description"", ""Type"", ""Deleted"") VALUES('CRD', 'Card', 1, false);");
            migrationBuilder.Sql(@"INSERT INTO ""UnitOfMeasures"" (""Code"", ""Description"", ""Type"", ""Deleted"") VALUES('CM', 'Centimeters', 1, false);");
            migrationBuilder.Sql(@"INSERT INTO ""UnitOfMeasures"" (""Code"", ""Description"", ""Type"", ""Deleted"") VALUES('CS', 'Case', 1, false);");
            migrationBuilder.Sql(@"INSERT INTO ""UnitOfMeasures"" (""Code"", ""Description"", ""Type"", ""Deleted"") VALUES('CTN', 'Carton', 1, false);");
            migrationBuilder.Sql(@"INSERT INTO ""UnitOfMeasures"" (""Code"", ""Description"", ""Type"", ""Deleted"") VALUES('DZ', 'Dozen', 1, false);");
            migrationBuilder.Sql(@"INSERT INTO ""UnitOfMeasures"" (""Code"", ""Description"", ""Type"", ""Deleted"") VALUES('EA', 'Each', 1, false);");
            migrationBuilder.Sql(@"INSERT INTO ""UnitOfMeasures"" (""Code"", ""Description"", ""Type"", ""Deleted"") VALUES('FT', 'Foot', 1, false);");
            migrationBuilder.Sql(@"INSERT INTO ""UnitOfMeasures"" (""Code"", ""Description"", ""Type"", ""Deleted"") VALUES('GAL', 'Gallon', 1, false);");
            migrationBuilder.Sql(@"INSERT INTO ""UnitOfMeasures"" (""Code"", ""Description"", ""Type"", ""Deleted"") VALUES('GROSS', 'Gross', 1, false);");
            migrationBuilder.Sql(@"INSERT INTO ""UnitOfMeasures"" (""Code"", ""Description"", ""Type"", ""Deleted"") VALUES('IN', 'Inches', 1, false);");
            migrationBuilder.Sql(@"INSERT INTO ""UnitOfMeasures"" (""Code"", ""Description"", ""Type"", ""Deleted"") VALUES('KIT', 'Kit', 1, false);");
            migrationBuilder.Sql(@"INSERT INTO ""UnitOfMeasures"" (""Code"", ""Description"", ""Type"", ""Deleted"") VALUES('LOT', 'Lot', 1, false);");
            migrationBuilder.Sql(@"INSERT INTO ""UnitOfMeasures"" (""Code"", ""Description"", ""Type"", ""Deleted"") VALUES('M', 'Meter', 1, false);");
            migrationBuilder.Sql(@"INSERT INTO ""UnitOfMeasures"" (""Code"", ""Description"", ""Type"", ""Deleted"") VALUES('MM', 'Millimeter', 1, false);");
            migrationBuilder.Sql(@"INSERT INTO ""UnitOfMeasures"" (""Code"", ""Description"", ""Type"", ""Deleted"") VALUES('PC', 'Piece', 1, false);");
            migrationBuilder.Sql(@"INSERT INTO ""UnitOfMeasures"" (""Code"", ""Description"", ""Type"", ""Deleted"") VALUES('PK', 'Pack', 1, false);");
            migrationBuilder.Sql(@"INSERT INTO ""UnitOfMeasures"" (""Code"", ""Description"", ""Type"", ""Deleted"") VALUES('PK100', 'Pack 100', 1, false);");
            migrationBuilder.Sql(@"INSERT INTO ""UnitOfMeasures"" (""Code"", ""Description"", ""Type"", ""Deleted"") VALUES('PK50', 'Pack 50', 1, false);");
            migrationBuilder.Sql(@"INSERT INTO ""UnitOfMeasures"" (""Code"", ""Description"", ""Type"", ""Deleted"") VALUES('PR', 'Pair', 1, false);");
            migrationBuilder.Sql(@"INSERT INTO ""UnitOfMeasures"" (""Code"", ""Description"", ""Type"", ""Deleted"") VALUES('RACK', 'Rack', 1, false);");
            migrationBuilder.Sql(@"INSERT INTO ""UnitOfMeasures"" (""Code"", ""Description"", ""Type"", ""Deleted"") VALUES('RL', 'Roll', 1, false);");
            migrationBuilder.Sql(@"INSERT INTO ""UnitOfMeasures"" (""Code"", ""Description"", ""Type"", ""Deleted"") VALUES('SET', 'Set', 1, false);");
            migrationBuilder.Sql(@"INSERT INTO ""UnitOfMeasures"" (""Code"", ""Description"", ""Type"", ""Deleted"") VALUES('SET3', 'Set of 3', 1, false);");
            migrationBuilder.Sql(@"INSERT INTO ""UnitOfMeasures"" (""Code"", ""Description"", ""Type"", ""Deleted"") VALUES('SET4', 'Set of 4', 1, false);");
            migrationBuilder.Sql(@"INSERT INTO ""UnitOfMeasures"" (""Code"", ""Description"", ""Type"", ""Deleted"") VALUES('SET5', 'Set of 5', 1, false);");
            migrationBuilder.Sql(@"INSERT INTO ""UnitOfMeasures"" (""Code"", ""Description"", ""Type"", ""Deleted"") VALUES('SGL', 'Single', 1, false);");
            migrationBuilder.Sql(@"INSERT INTO ""UnitOfMeasures"" (""Code"", ""Description"", ""Type"", ""Deleted"") VALUES('SHT', 'Sheet', 1, false);");
            migrationBuilder.Sql(@"INSERT INTO ""UnitOfMeasures"" (""Code"", ""Description"", ""Type"", ""Deleted"") VALUES('SQFT', 'Square ft', 1, false);");
            migrationBuilder.Sql(@"INSERT INTO ""UnitOfMeasures"" (""Code"", ""Description"", ""Type"", ""Deleted"") VALUES('TUBE', 'Tube', 1, false);");
            migrationBuilder.Sql(@"INSERT INTO ""UnitOfMeasures"" (""Code"", ""Description"", ""Type"", ""Deleted"") VALUES('YD', 'Yard', 1, false);");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"TRUNCATE TABLE ""UnitOfMeasures"" CASCADE;");
        }
    }
}
