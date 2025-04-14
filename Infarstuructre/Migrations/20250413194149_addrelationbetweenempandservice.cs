using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infarstuructre.Migrations
{
    public partial class addrelationbetweenempandservice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_employeeRequests_employees_EmployeeId",
                table: "employeeRequests");

            migrationBuilder.DropIndex(
                name: "IX_employeeRequests_EmployeeId",
                table: "employeeRequests");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "employees",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "SeID",
                table: "employees",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_employees_SeID",
                table: "employees",
                column: "SeID");

            migrationBuilder.AddForeignKey(
                name: "FK_employees_services_SeID",
                table: "employees",
                column: "SeID",
                principalTable: "services",
                principalColumn: "ServiceID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_employees_services_SeID",
                table: "employees");

            migrationBuilder.DropIndex(
                name: "IX_employees_SeID",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "SeID",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "employees");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_employeeRequests_EmployeeId",
                table: "employeeRequests",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_employeeRequests_employees_EmployeeId",
                table: "employeeRequests",
                column: "EmployeeId",
                principalTable: "employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
