using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacciStats.Migrations
{
    /// <inheritdoc />
    public partial class CorregirImagenesEjemplo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Personajes",
                keyColumn: "Id",
                keyValue: 106,
                column: "Image",
                value: "https://commons.wikimedia.org/wiki/Special:Redirect/file/Leia_Organa_Solo_Skywalker.jpg?width=500");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Personajes",
                keyColumn: "Id",
                keyValue: 106,
                column: "Image",
                value: "https://commons.wikimedia.org/wiki/Special:Redirect/file/Leia_Organa_%28Star_Wars%29.jpg?width=500");
        }
    }
}
