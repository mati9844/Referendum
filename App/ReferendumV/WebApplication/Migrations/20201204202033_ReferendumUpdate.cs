using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class ReferendumUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VoteId",
                table: "Questions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Questions_VoteId",
                table: "Questions",
                column: "VoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Votes_VoteId",
                table: "Questions",
                column: "VoteId",
                principalTable: "Votes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Votes_VoteId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_VoteId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "VoteId",
                table: "Questions");
        }
    }
}
