using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendMagaRace.Migrations
{
    /// <inheritdoc />
    public partial class AddRaceStatusToOnlineRace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFinished",
                table: "OnlineRaces");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "OnlineRaces",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "OnlineRaces");

            migrationBuilder.AddColumn<bool>(
                name: "IsFinished",
                table: "OnlineRaces",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
