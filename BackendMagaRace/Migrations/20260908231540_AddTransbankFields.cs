using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendMagaRace.Migrations
{
    /// <inheritdoc />
    public partial class AddTransbankFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CardTypeCode",
                table: "Deposits",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransbankFormUrl",
                table: "Deposits",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CardTypeCode",
                table: "Deposits");

            migrationBuilder.DropColumn(
                name: "TransbankFormUrl",
                table: "Deposits");
        }
    }
}
