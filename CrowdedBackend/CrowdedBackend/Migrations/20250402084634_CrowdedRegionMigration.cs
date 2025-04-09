using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CrowdedBackend.Migrations
{
    /// <inheritdoc />
    public partial class CrowdedRegionMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create the table 'Venue'
            migrationBuilder.CreateTable(
                name: "Venue",
                columns: table => new
                {
                    VenueID = table.Column<int>(type: "integer")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VenueName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Venue", x => x.VenueID);
                }
            );

            // Create the table 'DetectedDevice'
            migrationBuilder.CreateTable(
                name: "DetectedDevice",
                columns: table => new
                {
                    detectedDeviceId = table.Column<int>(type: "integer")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    venueID = table.Column<int>(type: "integer", nullable: false),
                    deviceX = table.Column<int>(type: "integer", nullable: false),
                    deviceY = table.Column<int>(type: "integer", nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetectedDevice", x => x.detectedDeviceId);
                    table.ForeignKey(
                        name: "FK_DetectedDevice_Venue_venueID",
                        column: x => x.venueID,
                        principalTable: "Venue",
                        principalColumn: "VenueID",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            // Create the table 'RaspberryPi'
            migrationBuilder.CreateTable(
                name: "RaspberryPi",
                columns: table => new
                {
                    RaspberryPiID = table.Column<int>(type: "integer")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VenueID = table.Column<int>(type: "integer", nullable: false),
                    raspX = table.Column<int>(type: "integer", nullable: false),
                    raspY = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaspberryPi", x => x.RaspberryPiID);
                    table.ForeignKey(
                        name: "FK_RaspberryPi_Venue_VenueID",
                        column: x => x.VenueID,
                        principalTable: "Venue",
                        principalColumn: "VenueID",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "RaspberryPi");
            migrationBuilder.DropTable(name: "DetectedDevice");
            migrationBuilder.DropTable(name: "Venue");
        }
    }
}
