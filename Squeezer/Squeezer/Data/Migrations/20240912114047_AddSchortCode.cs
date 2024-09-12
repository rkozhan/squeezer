﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Squeezer.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSchortCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Links_ShortCode",
                table: "Links",
                column: "ShortCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Links_ShortCode",
                table: "Links");
        }
    }
}
