using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendMagaRace.Migrations
{
    /// <inheritdoc />
    public partial class AddDatesToOnlineRace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledAt",
                table: "OnlineRaces",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "OnlineRaces",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AddColumn<DateTime>(
                name: "FinishedAt",
                table: "OnlineRaces",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartedAt",
                table: "OnlineRaces",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelledAt",
                table: "OnlineRaces");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "OnlineRaces");

            migrationBuilder.DropColumn(
                name: "FinishedAt",
                table: "OnlineRaces");

            migrationBuilder.DropColumn(
                name: "StartedAt",
                table: "OnlineRaces");
        }
    }
}
