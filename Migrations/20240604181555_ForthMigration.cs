using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RailConnect.Migrations
{
    /// <inheritdoc />
    public partial class ForthMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_StanicaDolazak_IdGrad",
                table: "StanicaDolazak",
                column: "IdGrad");

            migrationBuilder.AddForeignKey(
                name: "FK_StanicaDolazak_Grad_IdGrad",
                table: "StanicaDolazak",
                column: "IdGrad",
                principalTable: "Grad",
                principalColumn: "idGrad",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StanicaDolazak_Grad_IdGrad",
                table: "StanicaDolazak");

            migrationBuilder.DropIndex(
                name: "IX_StanicaDolazak_IdGrad",
                table: "StanicaDolazak");
        }
    }
}
