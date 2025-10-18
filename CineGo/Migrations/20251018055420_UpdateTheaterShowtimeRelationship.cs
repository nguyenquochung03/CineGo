using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineGo.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTheaterShowtimeRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Showtimes_Theaters_TheaterId",
                table: "Showtimes");

            migrationBuilder.DropIndex(
                name: "IX_Showtimes_Date_TheaterId",
                table: "Showtimes");

            migrationBuilder.DropIndex(
                name: "IX_Showtimes_TheaterId",
                table: "Showtimes");

            migrationBuilder.DropIndex(
                name: "IX_Showtimes_TheaterId_MovieId_Date_StartTime",
                table: "Showtimes");

            migrationBuilder.DropColumn(
                name: "TheaterId",
                table: "Showtimes");

            migrationBuilder.CreateTable(
                name: "TheaterShowtimes",
                columns: table => new
                {
                    TheaterId = table.Column<int>(type: "int", nullable: false),
                    ShowtimeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TheaterShowtimes", x => new { x.TheaterId, x.ShowtimeId });
                    table.ForeignKey(
                        name: "FK_TheaterShowtimes_Showtimes_ShowtimeId",
                        column: x => x.ShowtimeId,
                        principalTable: "Showtimes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TheaterShowtimes_Theaters_TheaterId",
                        column: x => x.TheaterId,
                        principalTable: "Theaters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Showtimes_Date_PricingRuleId",
                table: "Showtimes",
                columns: new[] { "Date", "PricingRuleId" });

            migrationBuilder.CreateIndex(
                name: "IX_TheaterShowtimes_ShowtimeId",
                table: "TheaterShowtimes",
                column: "ShowtimeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TheaterShowtimes");

            migrationBuilder.DropIndex(
                name: "IX_Showtimes_Date_PricingRuleId",
                table: "Showtimes");

            migrationBuilder.AddColumn<int>(
                name: "TheaterId",
                table: "Showtimes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Showtimes_Date_TheaterId",
                table: "Showtimes",
                columns: new[] { "Date", "TheaterId" });

            migrationBuilder.CreateIndex(
                name: "IX_Showtimes_TheaterId",
                table: "Showtimes",
                column: "TheaterId");

            migrationBuilder.CreateIndex(
                name: "IX_Showtimes_TheaterId_MovieId_Date_StartTime",
                table: "Showtimes",
                columns: new[] { "TheaterId", "MovieId", "Date", "StartTime" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Showtimes_Theaters_TheaterId",
                table: "Showtimes",
                column: "TheaterId",
                principalTable: "Theaters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
