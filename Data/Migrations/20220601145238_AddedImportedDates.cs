using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExcelManipulation.Data.Migrations
{
    public partial class AddedImportedDates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ImportedDate",
                table: "Employees",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImportedDate",
                table: "Employees");
        }
    }
}
