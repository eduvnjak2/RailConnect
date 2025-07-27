using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RailConnect.Migrations
{
    /// <inheritdoc />
    public partial class FInalMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Putovanje_StanicaDolazak_MjestoDolaska",
                table: "Putovanje");

            migrationBuilder.DropForeignKey(
                name: "FK_Putovanje_StanicaPolazak_MjestoPolaska",
                table: "Putovanje");

            migrationBuilder.DropIndex(
                name: "IX_Putovanje_MjestoDolaska",
                table: "Putovanje");

            migrationBuilder.DropIndex(
                name: "IX_Putovanje_MjestoPolaska",
                table: "Putovanje");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Putovanje_MjestoDolaska",
                table: "Putovanje",
                column: "MjestoDolaska");

            migrationBuilder.CreateIndex(
                name: "IX_Putovanje_MjestoPolaska",
                table: "Putovanje",
                column: "MjestoPolaska");

            migrationBuilder.AddForeignKey(
                name: "FK_Putovanje_StanicaDolazak_MjestoDolaska",
                table: "Putovanje",
                column: "MjestoDolaska",
                principalTable: "StanicaDolazak",
                principalColumn: "IdStanicaDolazak",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Putovanje_StanicaPolazak_MjestoPolaska",
                table: "Putovanje",
                column: "MjestoPolaska",
                principalTable: "StanicaPolazak",
                principalColumn: "IdStanicaPolazak",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
