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
            migrationBuilder.CreateTable(
                name: "HolidayPackages",
                columns: table => new
                {
                    HolidayPackageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HolidayPackages", x => x.HolidayPackageId);
                });

            migrationBuilder.CreateTable(
                name: "HolidayBookings",
                columns: table => new
                {
                    HolidayBookingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingReference = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    HolidayPackageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HolidayBookings", x => x.HolidayBookingId);
                    table.ForeignKey(
                        name: "FK_HolidayBookings_HolidayPackages_HolidayPackageId",
                        column: x => x.HolidayPackageId,
                        principalTable: "HolidayPackages",
                        principalColumn: "HolidayPackageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Travellers",
                columns: table => new
                {
                    TravellerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    PassportNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HolidayBookingId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Travellers", x => x.TravellerId);
                    table.ForeignKey(
                        name: "FK_Travellers_HolidayBookings_HolidayBookingId",
                        column: x => x.HolidayBookingId,
                        principalTable: "HolidayBookings",
                        principalColumn: "HolidayBookingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HolidayBookings_HolidayPackageId",
                table: "HolidayBookings",
                column: "HolidayPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Travellers_HolidayBookingId",
                table: "Travellers",
                column: "HolidayBookingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Travellers");

            migrationBuilder.DropTable(
                name: "HolidayBookings");

            migrationBuilder.DropTable(
                name: "HolidayPackages");
        }
    }
}
