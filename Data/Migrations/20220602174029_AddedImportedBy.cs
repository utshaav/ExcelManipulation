using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExcelManipulation.Data.Migrations
{
    public partial class AddedImportedBy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ImportedBy",
                table: "Employees",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImportedBy",
                table: "Employees");
        }
    }
}
