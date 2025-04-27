using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gotorz.Server.Migrations.GotorzDb
{
    /// <inheritdoc />
    public partial class AddMaxCapacityToHolidayPackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxCapacity",
                table: "HolidayPackages",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxCapacity",
                table: "HolidayPackages");
        }
    }
}
