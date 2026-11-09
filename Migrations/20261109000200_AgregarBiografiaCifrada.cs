using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
namespace FacciStats.Migrations;

[DbContext(typeof(FacciStats.AppDbContext))]
[Migration("20261109000200_AgregarBiografiaCifrada")]
public partial class AgregarBiografiaCifrada : Migration { protected override void Up(MigrationBuilder m) => m.AddColumn<string>(name: "BiografiaSecreta", table: "Personajes", maxLength: 2000, nullable: true); protected override void Down(MigrationBuilder m) => m.DropColumn("BiografiaSecreta", "Personajes"); }
