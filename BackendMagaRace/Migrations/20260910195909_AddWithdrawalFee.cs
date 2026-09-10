using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendMagaRace.Migrations
{
    /// <inheritdoc />
    public partial class AddWithdrawalFee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "FeeUsdt",
                table: "Withdrawals",
                type: "numeric(18,8)",
                precision: 18,
                scale: 8,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NetUsdt",
                table: "Withdrawals",
                type: "numeric(18,8)",
                precision: 18,
                scale: 8,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FeeUsdt",
                table: "Withdrawals");

            migrationBuilder.DropColumn(
                name: "NetUsdt",
                table: "Withdrawals");
        }
    }
}
