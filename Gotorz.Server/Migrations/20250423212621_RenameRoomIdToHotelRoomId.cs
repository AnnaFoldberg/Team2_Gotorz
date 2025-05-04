using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gotorz.Server.Migrations
{
    public partial class RenameRoomIdToHotelRoomId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HotelBookings_HotelRooms_RoomId",
                table: "HotelBookings");

            migrationBuilder.RenameColumn(
                name: "RoomId",
                table: "HotelBookings",
                newName: "HotelRoomId");

            migrationBuilder.RenameIndex(
                name: "IX_HotelBookings_RoomId",
                table: "HotelBookings",
                newName: "IX_HotelBookings_HotelRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_HotelBookings_HotelRooms_HotelRoomId",
                table: "HotelBookings",
                column: "HotelRoomId",
                principalTable: "HotelRooms",
                principalColumn: "HotelRoomId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HotelBookings_HotelRooms_HotelRoomId",
                table: "HotelBookings");

            migrationBuilder.RenameColumn(
                name: "HotelRoomId",
                table: "HotelBookings",
                newName: "RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_HotelBookings_HotelRoomId",
                table: "HotelBookings",
                newName: "IX_HotelBookings_RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_HotelBookings_HotelRooms_RoomId",
                table: "HotelBookings",
                column: "RoomId",
                principalTable: "HotelRooms",
                principalColumn: "HotelRoomId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
