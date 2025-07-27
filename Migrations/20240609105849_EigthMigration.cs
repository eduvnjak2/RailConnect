using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RailConnect.Migrations
{
    /// <inheritdoc />
    public partial class EigthMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recenzija_AspNetUsers_PutnikId",
                table: "Recenzija");

            migrationBuilder.DropForeignKey(
                name: "FK_StanicaPutovanje_StanicaDolazak_IdStanicaDolazak",
                table: "StanicaPutovanje");

            migrationBuilder.DropIndex(
                name: "IX_StanicaPutovanje_IdStanicaDolazak",
                table: "StanicaPutovanje");

            migrationBuilder.DropIndex(
                name: "IX_Recenzija_PutnikId",
                table: "Recenzija");

            migrationBuilder.AlterColumn<string>(
                name: "PutnikId",
                table: "Recenzija",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PutnikId",
                table: "Recenzija",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_StanicaPutovanje_IdStanicaDolazak",
                table: "StanicaPutovanje",
                column: "IdStanicaDolazak");

            migrationBuilder.CreateIndex(
                name: "IX_Recenzija_PutnikId",
                table: "Recenzija",
                column: "PutnikId");

            migrationBuilder.AddForeignKey(
                name: "FK_Recenzija_AspNetUsers_PutnikId",
                table: "Recenzija",
                column: "PutnikId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StanicaPutovanje_StanicaDolazak_IdStanicaDolazak",
                table: "StanicaPutovanje",
                column: "IdStanicaDolazak",
                principalTable: "StanicaDolazak",
                principalColumn: "IdStanicaDolazak",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
