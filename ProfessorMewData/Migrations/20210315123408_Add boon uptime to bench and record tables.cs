using Microsoft.EntityFrameworkCore.Migrations;

namespace ProfessorMewData.Migrations
{
    public partial class Addboonuptimetobenchandrecordtables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "BoonUptime",
                table: "Records",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BoonUptime",
                table: "RaidBenches",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoonUptime",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "BoonUptime",
                table: "RaidBenches");
        }
    }
}
