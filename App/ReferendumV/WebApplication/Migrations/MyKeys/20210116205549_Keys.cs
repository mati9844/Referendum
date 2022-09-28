using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations.MyKeys
{
    public partial class Keys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                       name: "DataProtectionKeys");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
