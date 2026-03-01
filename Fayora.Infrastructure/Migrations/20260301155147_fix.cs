using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fayora.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "VerificationCodes",
                newName: "Purpose");

            migrationBuilder.RenameIndex(
                name: "IX_VerificationCodes_Target_Type",
                table: "VerificationCodes",
                newName: "IX_VerificationCodes_Target_Purpose");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Purpose",
                table: "VerificationCodes",
                newName: "Type");

            migrationBuilder.RenameIndex(
                name: "IX_VerificationCodes_Target_Purpose",
                table: "VerificationCodes",
                newName: "IX_VerificationCodes_Target_Type");
        }
    }
}
