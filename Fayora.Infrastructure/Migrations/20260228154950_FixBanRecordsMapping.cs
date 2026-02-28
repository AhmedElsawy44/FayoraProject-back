using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fayora.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixBanRecordsMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BanRecords_Users_UserId",
                table: "BanRecords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BanRecords",
                table: "BanRecords");

            migrationBuilder.RenameTable(
                name: "BanRecords",
                newName: "BannedItems");

            migrationBuilder.RenameIndex(
                name: "IX_BanRecords_UserId",
                table: "BannedItems",
                newName: "IX_BannedItems_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_BanRecords_BanType_BanValue",
                table: "BannedItems",
                newName: "IX_BannedItems_BanType_BanValue");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BannedItems",
                table: "BannedItems",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BannedItems_Users_UserId",
                table: "BannedItems",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BannedItems_Users_UserId",
                table: "BannedItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BannedItems",
                table: "BannedItems");

            migrationBuilder.RenameTable(
                name: "BannedItems",
                newName: "BanRecords");

            migrationBuilder.RenameIndex(
                name: "IX_BannedItems_UserId",
                table: "BanRecords",
                newName: "IX_BanRecords_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_BannedItems_BanType_BanValue",
                table: "BanRecords",
                newName: "IX_BanRecords_BanType_BanValue");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BanRecords",
                table: "BanRecords",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BanRecords_Users_UserId",
                table: "BanRecords",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
