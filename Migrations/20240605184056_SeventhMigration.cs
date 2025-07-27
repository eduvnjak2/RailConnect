using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RailConnect.Migrations
{
    /// <inheritdoc />
    public partial class SeventhMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Karta_AspNetUsers_PutnikId",
                table: "Karta");

            migrationBuilder.DropForeignKey(
                name: "FK_Karta_StanicaDolazak_IdStanicaDolazak",
                table: "Karta");

            migrationBuilder.DropForeignKey(
                name: "FK_Karta_StanicaPolazak_IdStanicaPolazak",
                table: "Karta");

            migrationBuilder.DropIndex(
                name: "IX_Karta_IdStanicaDolazak",
                table: "Karta");

            migrationBuilder.DropIndex(
                name: "IX_Karta_IdStanicaPolazak",
                table: "Karta");

            migrationBuilder.DropIndex(
                name: "IX_Karta_PutnikId",
                table: "Karta");

            migrationBuilder.AlterColumn<string>(
                name: "PutnikId",
                table: "Karta",
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
                table: "Karta",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Karta_IdStanicaDolazak",
                table: "Karta",
                column: "IdStanicaDolazak");

            migrationBuilder.CreateIndex(
                name: "IX_Karta_IdStanicaPolazak",
                table: "Karta",
                column: "IdStanicaPolazak");

            migrationBuilder.CreateIndex(
                name: "IX_Karta_PutnikId",
                table: "Karta",
                column: "PutnikId");

            migrationBuilder.AddForeignKey(
                name: "FK_Karta_AspNetUsers_PutnikId",
                table: "Karta",
                column: "PutnikId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Karta_StanicaDolazak_IdStanicaDolazak",
                table: "Karta",
                column: "IdStanicaDolazak",
                principalTable: "StanicaDolazak",
                principalColumn: "IdStanicaDolazak",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Karta_StanicaPolazak_IdStanicaPolazak",
                table: "Karta",
                column: "IdStanicaPolazak",
                principalTable: "StanicaPolazak",
                principalColumn: "IdStanicaPolazak",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
