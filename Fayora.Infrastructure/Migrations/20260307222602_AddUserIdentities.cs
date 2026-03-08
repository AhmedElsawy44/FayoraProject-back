using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fayora.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdentities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserIdentities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    LinkedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserIdentities", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserIdentities_Email",
                table: "UserIdentities",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_UserIdentities_Provider_ProviderKey",
                table: "UserIdentities",
                columns: new[] { "Provider", "ProviderKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserIdentities_UserId_Provider",
                table: "UserIdentities",
                columns: new[] { "UserId", "Provider" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserIdentities");
        }
    }
}
