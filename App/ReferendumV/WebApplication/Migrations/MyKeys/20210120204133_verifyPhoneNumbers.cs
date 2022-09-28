using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations.MyKeys
{
    public partial class verifyPhoneNumbers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "verifyPhoneNumbers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnvelopeID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateTimeSent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateTimeConfirmation = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_verifyPhoneNumbers", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropTable(
                name: "verifyPhoneNumbers");
        }
    }
}
