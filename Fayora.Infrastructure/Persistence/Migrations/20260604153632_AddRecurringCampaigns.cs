using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fayora.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRecurringCampaigns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CronExpression",
                table: "PushCampaigns",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRecurring",
                table: "PushCampaigns",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastRunAt",
                table: "PushCampaigns",
                type: "datetimeoffset",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CronExpression",
                table: "PushCampaigns");

            migrationBuilder.DropColumn(
                name: "IsRecurring",
                table: "PushCampaigns");

            migrationBuilder.DropColumn(
                name: "LastRunAt",
                table: "PushCampaigns");
        }
    }
}
