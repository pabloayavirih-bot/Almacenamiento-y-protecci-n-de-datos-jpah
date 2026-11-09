using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FacciStats.Migrations
{
    /// <inheritdoc />
    public partial class RestaurarCatalogoPersonajes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BiografiaSecreta",
                table: "Personajes");

            migrationBuilder.AddColumn<string>(
                name: "Afiliacion",
                table: "Personajes",
                type: "varchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Especie",
                table: "Personajes",
                type: "varchar(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Personajes",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "desconocido")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Personajes",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "FuerzaSensitivo",
                table: "Personajes",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql("UPDATE `Personajes` SET `Estado` = CASE WHEN `EstaMuerto` = 1 THEN 'muerto' ELSE 'vivo' END;");

            migrationBuilder.DropColumn(
                name: "EstaMuerto",
                table: "Personajes");

            migrationBuilder.InsertData(
                table: "Eventos",
                columns: new[] { "Id", "FaccionGanadora", "Fecha", "Nombre" },
                values: new object[,]
                {
                    { 101, "Rebelde", new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Batalla de Yavin" },
                    { 102, "Imperio", new DateTime(2024, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Batalla de Hoth" },
                    { 103, "Rebelde", new DateTime(2024, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Misión en Endor" },
                    { 104, "Neutral", new DateTime(2024, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Consejo de la Fuerza" }
                });

            migrationBuilder.InsertData(
                table: "Personajes",
                columns: new[] { "Id", "Afiliacion", "Especie", "Estado", "Faccion", "FuerzaSensitivo", "Image", "Nombre" },
                values: new object[,]
                {
                    { 101, "Alianza Rebelde", "Humano", "vivo", "Rebelde", true, "https://commons.wikimedia.org/wiki/Special:Redirect/file/SWCA_-_Luke_Skywalker_(16580421784).jpg?width=500", "Luke Skywalker" },
                    { 102, "Imperio Galáctico", "Humano", "muerto", "Imperio", true, "https://commons.wikimedia.org/wiki/Special:Redirect/file/Darth_Vader_at_Galaxy%E2%80%99s_Edge.jpg?width=500", "Darth Vader" },
                    { 103, "Alianza Rebelde", "Humano", "vivo", "Rebelde", false, "https://commons.wikimedia.org/wiki/Special:Redirect/file/Han_Solo_in_Carbonite_(31649435213).jpg?width=500", "Han Solo" },
                    { 104, "Alianza Rebelde", "Wookiee", "vivo", "Rebelde", false, "https://commons.wikimedia.org/wiki/Special:Redirect/file/Chewbacca_(7865110592).jpg?width=500", "Chewbacca" },
                    { 105, "Orden Jedi", "Desconocida", "muerto", "Neutral", true, "https://commons.wikimedia.org/wiki/Special:Redirect/file/Yoda_I_know_you%27re_in_there%21.jpg?width=500", "Yoda" },
                    { 106, "Alianza Rebelde", "Humana", "vivo", "Rebelde", false, "https://commons.wikimedia.org/wiki/Special:Redirect/file/Leia_Organa_%28Star_Wars%29.jpg?width=500", "Leia Organa" }
                });

            migrationBuilder.InsertData(
                table: "Cartas",
                columns: new[] { "Id", "Nombre", "PersonajeId", "Poder" },
                values: new object[,]
                {
                    { 101, "Carta de Luke", 101, 92 },
                    { 102, "Carta de Vader", 102, 95 },
                    { 103, "Carta de Han", 103, 76 },
                    { 104, "Carta de Chewbacca", 104, 82 },
                    { 105, "Carta de Yoda", 105, 98 },
                    { 106, "Carta de Leia", 106, 88 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Cartas",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Cartas",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "Cartas",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "Cartas",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "Cartas",
                keyColumn: "Id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "Cartas",
                keyColumn: "Id",
                keyValue: 106);

            migrationBuilder.DeleteData(
                table: "Eventos",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Eventos",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "Eventos",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "Eventos",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "Personajes",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Personajes",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "Personajes",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "Personajes",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "Personajes",
                keyColumn: "Id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "Personajes",
                keyColumn: "Id",
                keyValue: 106);

            migrationBuilder.AddColumn<bool>(
                name: "EstaMuerto",
                table: "Personajes",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql("UPDATE `Personajes` SET `EstaMuerto` = CASE WHEN `Estado` = 'muerto' THEN 1 ELSE 0 END;");

            migrationBuilder.DropColumn(
                name: "FuerzaSensitivo",
                table: "Personajes");

            migrationBuilder.DropColumn(
                name: "Afiliacion",
                table: "Personajes");

            migrationBuilder.DropColumn(
                name: "Especie",
                table: "Personajes");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Personajes");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Personajes");

            migrationBuilder.AddColumn<string>(
                name: "BiografiaSecreta",
                table: "Personajes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
