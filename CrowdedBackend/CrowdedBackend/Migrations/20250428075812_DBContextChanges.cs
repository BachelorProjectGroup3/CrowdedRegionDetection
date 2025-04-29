using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrowdedBackend.Migrations
{
    /// <inheritdoc />
    public partial class DBContextChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetectedDevice_Venue_venueID",
                table: "DetectedDevice");

            migrationBuilder.RenameColumn(
                name: "raspId",
                table: "RaspData",
                newName: "RaspId");

            migrationBuilder.RenameColumn(
                name: "raspY",
                table: "RaspberryPi",
                newName: "RaspY");

            migrationBuilder.RenameColumn(
                name: "raspX",
                table: "RaspberryPi",
                newName: "RaspX");

            migrationBuilder.RenameColumn(
                name: "venueID",
                table: "DetectedDevice",
                newName: "VenueID");

            migrationBuilder.RenameColumn(
                name: "timestamp",
                table: "DetectedDevice",
                newName: "Timestamp");

            migrationBuilder.RenameColumn(
                name: "deviceY",
                table: "DetectedDevice",
                newName: "DeviceY");

            migrationBuilder.RenameColumn(
                name: "deviceX",
                table: "DetectedDevice",
                newName: "DeviceX");

            migrationBuilder.RenameColumn(
                name: "detectedDeviceId",
                table: "DetectedDevice",
                newName: "DetectedDeviceId");

            migrationBuilder.RenameIndex(
                name: "IX_DetectedDevice_venueID",
                table: "DetectedDevice",
                newName: "IX_DetectedDevice_VenueID");

            migrationBuilder.AddForeignKey(
                name: "FK_DetectedDevice_Venue_VenueID",
                table: "DetectedDevice",
                column: "VenueID",
                principalTable: "Venue",
                principalColumn: "VenueID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetectedDevice_Venue_VenueID",
                table: "DetectedDevice");

            migrationBuilder.RenameColumn(
                name: "RaspId",
                table: "RaspData",
                newName: "raspId");

            migrationBuilder.RenameColumn(
                name: "RaspY",
                table: "RaspberryPi",
                newName: "raspY");

            migrationBuilder.RenameColumn(
                name: "RaspX",
                table: "RaspberryPi",
                newName: "raspX");

            migrationBuilder.RenameColumn(
                name: "VenueID",
                table: "DetectedDevice",
                newName: "venueID");

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "DetectedDevice",
                newName: "timestamp");

            migrationBuilder.RenameColumn(
                name: "DeviceY",
                table: "DetectedDevice",
                newName: "deviceY");

            migrationBuilder.RenameColumn(
                name: "DeviceX",
                table: "DetectedDevice",
                newName: "deviceX");

            migrationBuilder.RenameColumn(
                name: "DetectedDeviceId",
                table: "DetectedDevice",
                newName: "detectedDeviceId");

            migrationBuilder.RenameIndex(
                name: "IX_DetectedDevice_VenueID",
                table: "DetectedDevice",
                newName: "IX_DetectedDevice_venueID");

            migrationBuilder.AddForeignKey(
                name: "FK_DetectedDevice_Venue_venueID",
                table: "DetectedDevice",
                column: "venueID",
                principalTable: "Venue",
                principalColumn: "VenueID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
