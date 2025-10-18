using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineGo.Migrations
{
    /// <inheritdoc />
    public partial class AddMoviePoster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MoviePoster_Movies_MovieId",
                table: "MoviePoster");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MoviePoster",
                table: "MoviePoster");

            migrationBuilder.RenameTable(
                name: "MoviePoster",
                newName: "MoviePosters");

            migrationBuilder.RenameIndex(
                name: "IX_MoviePoster_MovieId_Order",
                table: "MoviePosters",
                newName: "IX_MoviePosters_MovieId_Order");

            migrationBuilder.RenameIndex(
                name: "IX_MoviePoster_MovieId",
                table: "MoviePosters",
                newName: "IX_MoviePosters_MovieId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MoviePosters",
                table: "MoviePosters",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MoviePosters_Movies_MovieId",
                table: "MoviePosters",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MoviePosters_Movies_MovieId",
                table: "MoviePosters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MoviePosters",
                table: "MoviePosters");

            migrationBuilder.RenameTable(
                name: "MoviePosters",
                newName: "MoviePoster");

            migrationBuilder.RenameIndex(
                name: "IX_MoviePosters_MovieId_Order",
                table: "MoviePoster",
                newName: "IX_MoviePoster_MovieId_Order");

            migrationBuilder.RenameIndex(
                name: "IX_MoviePosters_MovieId",
                table: "MoviePoster",
                newName: "IX_MoviePoster_MovieId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MoviePoster",
                table: "MoviePoster",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MoviePoster_Movies_MovieId",
                table: "MoviePoster",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
