using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace yazlab2mvc.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCascadeBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Etkinlikler_Kullanicilar_OlusturanKullaniciID",
                table: "Etkinlikler");

            migrationBuilder.AddForeignKey(
                name: "FK_Etkinlikler_Kullanicilar_OlusturanKullaniciID",
                table: "Etkinlikler",
                column: "OlusturanKullaniciID",
                principalTable: "Kullanicilar",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Etkinlikler_Kullanicilar_OlusturanKullaniciID",
                table: "Etkinlikler");

            migrationBuilder.AddForeignKey(
                name: "FK_Etkinlikler_Kullanicilar_OlusturanKullaniciID",
                table: "Etkinlikler",
                column: "OlusturanKullaniciID",
                principalTable: "Kullanicilar",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
