using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendMagaRace.Migrations
{
    /// <inheritdoc />
    public partial class AddDatesToOnlineRace2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Position",
                table: "OnlineRacePlayers",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<DateTime>(
                name: "FinishedAt",
                table: "OnlineRacePlayers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsConnected",
                table: "OnlineRacePlayers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOwner",
                table: "OnlineRacePlayers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "JoinedAt",
                table: "OnlineRacePlayers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinishedAt",
                table: "OnlineRacePlayers");

            migrationBuilder.DropColumn(
                name: "IsConnected",
                table: "OnlineRacePlayers");

            migrationBuilder.DropColumn(
                name: "IsOwner",
                table: "OnlineRacePlayers");

            migrationBuilder.DropColumn(
                name: "JoinedAt",
                table: "OnlineRacePlayers");

            migrationBuilder.AlterColumn<int>(
                name: "Position",
                table: "OnlineRacePlayers",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
