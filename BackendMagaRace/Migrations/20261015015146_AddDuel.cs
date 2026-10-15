using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendMagaRace.Migrations
{
    /// <inheritdoc />
    public partial class AddDuel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Duels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatorUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    WagerUsdt = table.Column<decimal>(type: "numeric(18,8)", precision: 18, scale: 8, nullable: false),
                    RoomName = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FinishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Duels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DuelParticipants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DuelId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    FinalPosition = table.Column<int>(type: "integer", nullable: true),
                    Disconnected = table.Column<bool>(type: "boolean", nullable: false),
                    PrizeUsdt = table.Column<decimal>(type: "numeric(18,8)", precision: 18, scale: 8, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DuelParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DuelParticipants_Duels_DuelId",
                        column: x => x.DuelId,
                        principalTable: "Duels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DuelParticipants_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DuelParticipants_DuelId_UserId",
                table: "DuelParticipants",
                columns: new[] { "DuelId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DuelParticipants_UserId",
                table: "DuelParticipants",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DuelParticipants");

            migrationBuilder.DropTable(
                name: "Duels");
        }
    }
}
