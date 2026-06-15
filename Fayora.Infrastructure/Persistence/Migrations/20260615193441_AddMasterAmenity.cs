using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fayora.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMasterAmenity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amenities",
                table: "HousingUnits");

            migrationBuilder.CreateTable(
                name: "MasterAmenities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IconUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Category = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterAmenities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HousingUnitMasterAmenities",
                columns: table => new
                {
                    HousingUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MasterAmenityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HousingUnitMasterAmenities", x => new { x.HousingUnitId, x.MasterAmenityId });
                    table.ForeignKey(
                        name: "FK_HousingUnitMasterAmenities_HousingUnits_HousingUnitId",
                        column: x => x.HousingUnitId,
                        principalTable: "HousingUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HousingUnitMasterAmenities_MasterAmenities_MasterAmenityId",
                        column: x => x.MasterAmenityId,
                        principalTable: "MasterAmenities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HousingUnitMasterAmenities_MasterAmenityId",
                table: "HousingUnitMasterAmenities",
                column: "MasterAmenityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HousingUnitMasterAmenities");

            migrationBuilder.DropTable(
                name: "MasterAmenities");

            migrationBuilder.AddColumn<long>(
                name: "Amenities",
                table: "HousingUnits",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
