using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendMagaRace.Migrations
{
    /// <inheritdoc />
    public partial class AddQualiEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CarCategory",
                table: "QualifierEvents",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Direction",
                table: "QualifierEvents",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Laps",
                table: "QualifierEvents",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Transmission",
                table: "QualifierEvents",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarCategory",
                table: "QualifierEvents");

            migrationBuilder.DropColumn(
                name: "Direction",
                table: "QualifierEvents");

            migrationBuilder.DropColumn(
                name: "Laps",
                table: "QualifierEvents");

            migrationBuilder.DropColumn(
                name: "Transmission",
                table: "QualifierEvents");
        }
    }
}
