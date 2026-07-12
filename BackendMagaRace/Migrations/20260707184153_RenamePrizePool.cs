using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendMagaRace.Migrations
{
    /// <inheritdoc />
    public partial class RenamePrizePool : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrizePool",
                table: "QualifierEvents");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "QualifierSessions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<decimal>(
                name: "EntryCost",
                table: "QualifierEvents",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<decimal>(
                name: "BasePrize",
                table: "QualifierEvents",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "QualifierEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QualifierEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntryCost = table.Column<decimal>(type: "numeric", nullable: false),
                    PurchasedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActiveUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QualifierEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QualifierEntries_QualifierEvents_QualifierEventId",
                        column: x => x.QualifierEventId,
                        principalTable: "QualifierEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QualifierEntries_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QualifierPrizes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QualifierEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromPosition = table.Column<int>(type: "integer", nullable: false),
                    ToPosition = table.Column<int>(type: "integer", nullable: false),
                    FixedAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    PrizePercent = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QualifierPrizes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QualifierPrizes_QualifierEvents_QualifierEventId",
                        column: x => x.QualifierEventId,
                        principalTable: "QualifierEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QualifierEntries_QualifierEventId",
                table: "QualifierEntries",
                column: "QualifierEventId");

            migrationBuilder.CreateIndex(
                name: "IX_QualifierEntries_UserId",
                table: "QualifierEntries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_QualifierPrizes_QualifierEventId",
                table: "QualifierPrizes",
                column: "QualifierEventId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QualifierEntries");

            migrationBuilder.DropTable(
                name: "QualifierPrizes");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "QualifierSessions");

            migrationBuilder.DropColumn(
                name: "BasePrize",
                table: "QualifierEvents");

            migrationBuilder.AlterColumn<int>(
                name: "EntryCost",
                table: "QualifierEvents",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<int>(
                name: "PrizePool",
                table: "QualifierEvents",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
