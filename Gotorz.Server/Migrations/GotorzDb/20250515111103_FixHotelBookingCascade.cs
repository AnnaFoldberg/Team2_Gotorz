using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gotorz.Server.Migrations.GotorzDb
{
    /// <inheritdoc />
    public partial class FixHotelBookingCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HotelBookingId",
                table: "HolidayBookings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HolidayBookings_HotelBookingId",
                table: "HolidayBookings",
                column: "HotelBookingId",
                unique: true,
                filter: "[HotelBookingId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_HolidayBookings_HotelBookings_HotelBookingId",
                table: "HolidayBookings",
                column: "HotelBookingId",
                principalTable: "HotelBookings",
                principalColumn: "HotelBookingId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HolidayBookings_HotelBookings_HotelBookingId",
                table: "HolidayBookings");

            migrationBuilder.DropIndex(
                name: "IX_HolidayBookings_HotelBookingId",
                table: "HolidayBookings");

            migrationBuilder.DropColumn(
                name: "HotelBookingId",
                table: "HolidayBookings");
        }
    }
}
