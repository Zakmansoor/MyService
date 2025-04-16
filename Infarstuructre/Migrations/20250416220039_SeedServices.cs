using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infarstuructre.Migrations
{
    public partial class SeedServices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NameAr",
                table: "services",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "services",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "services",
                columns: new[] { "ServiceID", "CreatedAt", "Description", "IsActive", "Name", "NameAr" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 4, 17, 1, 0, 39, 467, DateTimeKind.Local).AddTicks(212), "إصلاح الأعطال الكهربائية وتركيب الأنظمة", true, "Electrical Maintenance", "صيانة كهربائية" },
                    { 2, new DateTime(2025, 4, 17, 1, 0, 39, 467, DateTimeKind.Local).AddTicks(215), "إصلاح تسربات المياه وتركيب المواسير", true, "Plumbing", "سباكة" },
                    { 3, new DateTime(2025, 4, 17, 1, 0, 39, 467, DateTimeKind.Local).AddTicks(217), "تنظيف وصيانة أنظمة التبريد", true, "AC Maintenance", "صيانة تكييف" },
                    { 4, new DateTime(2025, 4, 17, 1, 0, 39, 467, DateTimeKind.Local).AddTicks(218), "إصلاح الغسالات والثلاجات", true, "Home Appliances", "أجهزة منزلية" },
                    { 5, new DateTime(2025, 4, 17, 1, 0, 39, 467, DateTimeKind.Local).AddTicks(220), "خدمات الدهان الداخلي والخارجي", true, "Painting", "دهان" },
                    { 6, new DateTime(2025, 4, 17, 1, 0, 39, 467, DateTimeKind.Local).AddTicks(222), "تصليح الأثاث وتركيب الأرضيات", true, "Carpentry", "نجارة" },
                    { 7, new DateTime(2025, 4, 17, 1, 0, 39, 467, DateTimeKind.Local).AddTicks(223), "إصلاح النوافذ المكسورة", true, "Glass Installation", "تركيب زجاج" },
                    { 8, new DateTime(2025, 4, 17, 1, 0, 39, 467, DateTimeKind.Local).AddTicks(225), "تركيب البلاط الجديد وإصلاح التالف", true, "Tiling", "تركيب بلاط" },
                    { 10, new DateTime(2025, 4, 17, 1, 0, 39, 467, DateTimeKind.Local).AddTicks(226), "تنظيف المنازل والمكاتب", true, "Cleaning Services", "خدمات تنظيف" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "services",
                keyColumn: "ServiceID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "services",
                keyColumn: "ServiceID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "services",
                keyColumn: "ServiceID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "services",
                keyColumn: "ServiceID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "services",
                keyColumn: "ServiceID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "services",
                keyColumn: "ServiceID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "services",
                keyColumn: "ServiceID",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "services",
                keyColumn: "ServiceID",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "services",
                keyColumn: "ServiceID",
                keyValue: 10);

            migrationBuilder.AlterColumn<string>(
                name: "NameAr",
                table: "services",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "services",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }
    }
}
