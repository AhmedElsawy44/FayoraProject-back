using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fayora.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupDiscountToGuidePackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GroupDiscountMinPeople",
                table: "GuideTourPackages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GroupDiscountPercent",
                table: "GuideTourPackages",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasGroupDiscount",
                table: "GuideTourPackages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupDiscountMinPeople",
                table: "GuideTourPackages");

            migrationBuilder.DropColumn(
                name: "GroupDiscountPercent",
                table: "GuideTourPackages");

            migrationBuilder.DropColumn(
                name: "HasGroupDiscount",
                table: "GuideTourPackages");
        }
    }
}
