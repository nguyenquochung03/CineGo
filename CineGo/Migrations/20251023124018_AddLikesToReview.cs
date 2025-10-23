﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineGo.Migrations
{
    /// <inheritdoc />
    public partial class AddLikesToReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Likes",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Likes",
                table: "Reviews");
        }
    }
}
