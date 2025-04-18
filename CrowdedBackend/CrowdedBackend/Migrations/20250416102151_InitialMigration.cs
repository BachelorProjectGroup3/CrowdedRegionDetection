using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CrowdedBackend.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Venue",
                columns: table => new
                {
                    VenueID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VenueName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Venue", x => x.VenueID);
                });

            migrationBuilder.CreateTable(
                name: "DetectedDevice",
                columns: table => new
                {
                    detectedDeviceId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    venueID = table.Column<int>(type: "integer", nullable: false),
                    deviceX = table.Column<double>(type: "double precision", nullable: false),
                    deviceY = table.Column<double>(type: "double precision", nullable: false),
                    timestamp = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetectedDevice", x => x.detectedDeviceId);
                    table.ForeignKey(
                        name: "FK_DetectedDevice_Venue_venueID",
                        column: x => x.venueID,
                        principalTable: "Venue",
                        principalColumn: "VenueID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RaspberryPi",
                columns: table => new
                {
                    RaspberryPiID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VenueID = table.Column<int>(type: "integer", nullable: false),
                    raspX = table.Column<double>(type: "double precision", nullable: false),
                    raspY = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaspberryPi", x => x.RaspberryPiID);
                    table.ForeignKey(
                        name: "FK_RaspberryPi_Venue_VenueID",
                        column: x => x.VenueID,
                        principalTable: "Venue",
                        principalColumn: "VenueID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DetectedDevice_venueID",
                table: "DetectedDevice",
                column: "venueID");

            migrationBuilder.CreateIndex(
                name: "IX_RaspberryPi_VenueID",
                table: "RaspberryPi",
                column: "VenueID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetectedDevice");

            migrationBuilder.DropTable(
                name: "RaspberryPi");

            migrationBuilder.DropTable(
                name: "Venue");
        }
    }
}
