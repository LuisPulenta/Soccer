using Microsoft.EntityFrameworkCore.Migrations;

namespace Soccer.Web.Migrations
{
    public partial class AddIndexInNameOnTeam : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Leagues_Name",
                table: "Leagues",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Leagues_Name",
                table: "Leagues");
        }
    }
}
