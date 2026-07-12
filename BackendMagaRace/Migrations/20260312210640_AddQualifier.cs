using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendMagaRace.Migrations
{
    /// <inheritdoc />
    public partial class AddQualifier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecondBestLapMs",
                table: "QualifierSessions");

            migrationBuilder.DropColumn(
                name: "ThirdBestLapMs",
                table: "QualifierSessions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SecondBestLapMs",
                table: "QualifierSessions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ThirdBestLapMs",
                table: "QualifierSessions",
                type: "integer",
                nullable: true);
        }
    }
}
