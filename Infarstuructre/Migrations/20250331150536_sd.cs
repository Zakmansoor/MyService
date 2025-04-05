using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infarstuructre.Migrations
{
    public partial class sd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "emali",
                table: "notifications");

            migrationBuilder.AddColumn<int>(
                name: "requestId",
                table: "notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_notifications_requestId",
                table: "notifications",
                column: "requestId");

            migrationBuilder.AddForeignKey(
                name: "FK_notifications_requests_requestId",
                table: "notifications",
                column: "requestId",
                principalTable: "requests",
                principalColumn: "RequestId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_notifications_requests_requestId",
                table: "notifications");

            migrationBuilder.DropIndex(
                name: "IX_notifications_requestId",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "requestId",
                table: "notifications");

            migrationBuilder.AddColumn<string>(
                name: "emali",
                table: "notifications",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
