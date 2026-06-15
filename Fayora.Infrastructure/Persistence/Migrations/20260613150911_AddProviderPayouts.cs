using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fayora.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProviderPayouts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPayoutProcessed",
                table: "Bookings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ProviderPayouts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PayoutDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderPayouts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_IsPayoutProcessed",
                table: "Bookings",
                column: "IsPayoutProcessed");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderPayouts_PayoutDate",
                table: "ProviderPayouts",
                column: "PayoutDate");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderPayouts_ProviderId",
                table: "ProviderPayouts",
                column: "ProviderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProviderPayouts");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_IsPayoutProcessed",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "IsPayoutProcessed",
                table: "Bookings");
        }
    }
}
