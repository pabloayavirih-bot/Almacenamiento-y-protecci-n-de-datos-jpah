using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacciStats.Migrations
{
    /// <inheritdoc />
    public partial class CompletarAfiliacionLuke : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Personajes",
                keyColumn: "Id",
                keyValue: 101,
                column: "Afiliacion",
                value: "Orden Jedi y Alianza Rebelde");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Personajes",
                keyColumn: "Id",
                keyValue: 101,
                column: "Afiliacion",
                value: "Alianza Rebelde");
        }
    }
}
