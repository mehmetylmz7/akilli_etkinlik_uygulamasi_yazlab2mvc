using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace yazlab2mvc.Migrations
{
    /// <inheritdoc />
    public partial class AddOlusturanKullaniciIDToEtkinlikler : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OlusturanKullaniciID",
                table: "Etkinlikler",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Etkinlikler_OlusturanKullaniciID",
                table: "Etkinlikler",
                column: "OlusturanKullaniciID");

            migrationBuilder.AddForeignKey(
                name: "FK_Etkinlikler_Kullanicilar_OlusturanKullaniciID",
                table: "Etkinlikler",
                column: "OlusturanKullaniciID",
                principalTable: "Kullanicilar",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Etkinlikler_Kullanicilar_OlusturanKullaniciID",
                table: "Etkinlikler");

            migrationBuilder.DropIndex(
                name: "IX_Etkinlikler_OlusturanKullaniciID",
                table: "Etkinlikler");

            migrationBuilder.DropColumn(
                name: "OlusturanKullaniciID",
                table: "Etkinlikler");
        }
    }
}
