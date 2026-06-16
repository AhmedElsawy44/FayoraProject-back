using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fayora.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPackageAccommodationAndNights : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NightIds",
                table: "GuideTourPackages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "PackageAccommodations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    MainImageUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    CheckInTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    CheckOutTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    Amenities = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageAccommodations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PackageNights",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NightNumber = table.Column<int>(type: "int", nullable: false),
                    NightDate = table.Column<DateOnly>(type: "date", nullable: false),
                    HousingUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PackageAccommodationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageNights", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PackageAccommodationGalleryImages",
                columns: table => new
                {
                    PackageAccommodationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageAccommodationGalleryImages", x => new { x.PackageAccommodationId, x.Id });
                    table.ForeignKey(
                        name: "FK_PackageAccommodationGalleryImages_PackageAccommodations_PackageAccommodationId",
                        column: x => x.PackageAccommodationId,
                        principalTable: "PackageAccommodations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PackageAccommodationGalleryImages");

            migrationBuilder.DropTable(
                name: "PackageNights");

            migrationBuilder.DropTable(
                name: "PackageAccommodations");

            migrationBuilder.DropColumn(
                name: "NightIds",
                table: "GuideTourPackages");
        }
    }
}
