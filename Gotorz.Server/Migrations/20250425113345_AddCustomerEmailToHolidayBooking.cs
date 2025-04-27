using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gotorz.Server.Migrations.GotorzDb
{
    /// <inheritdoc />
    public partial class AddCustomerEmailToHolidayBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerEmail",
                table: "HolidayBookings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerEmail",
                table: "HolidayBookings");
        }
    }
}
