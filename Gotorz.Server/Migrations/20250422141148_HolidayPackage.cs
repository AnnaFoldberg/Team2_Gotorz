using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gotorz.Server.Migrations.GotorzDb
{
    /// <inheritdoc />
    public partial class HolidayPackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HolidayPackageId",
                table: "Flights",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HolidayPackages",
                columns: table => new
                {
                    HolidayPackageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CostPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MarkupPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HolidayPackages", x => x.HolidayPackageId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Flights_HolidayPackageId",
                table: "Flights",
                column: "HolidayPackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Flights_HolidayPackages_HolidayPackageId",
                table: "Flights",
                column: "HolidayPackageId",
                principalTable: "HolidayPackages",
                principalColumn: "HolidayPackageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flights_HolidayPackages_HolidayPackageId",
                table: "Flights");

            migrationBuilder.DropTable(
                name: "HolidayPackages");

            migrationBuilder.DropIndex(
                name: "IX_Flights_HolidayPackageId",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "HolidayPackageId",
                table: "Flights");
        }
    }
}
