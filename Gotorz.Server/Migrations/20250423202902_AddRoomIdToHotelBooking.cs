using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gotorz.Server.Migrations
{
    public partial class AddRoomIdToHotelBooking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoomId",
                table: "HotelBookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_HotelBookings_RoomId",
                table: "HotelBookings",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_HotelBookings_HotelRooms_RoomId",
                table: "HotelBookings",
                column: "RoomId",
                principalTable: "HotelRooms",
                principalColumn: "HotelRoomId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HotelBookings_HotelRooms_RoomId",
                table: "HotelBookings");

            migrationBuilder.DropIndex(
                name: "IX_HotelBookings_RoomId",
                table: "HotelBookings");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "HotelBookings");
        }
    }
}
