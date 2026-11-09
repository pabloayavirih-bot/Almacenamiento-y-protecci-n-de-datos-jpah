using Microsoft.EntityFrameworkCore.Migrations;
namespace FacciStats.Migrations;
public partial class AgregarBiografiaCifrada : Migration { protected override void Up(MigrationBuilder m)=>m.AddColumn<string>(name:"BiografiaSecreta",table:"Personajes",maxLength:2000,nullable:true); protected override void Down(MigrationBuilder m)=>m.DropColumn("BiografiaSecreta","Personajes"); }
