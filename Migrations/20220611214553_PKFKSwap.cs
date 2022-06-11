using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExcelManipulation.Migrations
{
    public partial class PKFKSwap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photoes_Employees_EmployeeId",
                table: "Photoes");

            migrationBuilder.DropIndex(
                name: "IX_Photoes_EmployeeId",
                table: "Photoes");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Photoes");

            migrationBuilder.AddColumn<int>(
                name: "PhotoId",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PhotoId",
                table: "Employees",
                column: "PhotoId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Photoes_PhotoId",
                table: "Employees",
                column: "PhotoId",
                principalTable: "Photoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Photoes_PhotoId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_PhotoId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "PhotoId",
                table: "Employees");

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                table: "Photoes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Photoes_EmployeeId",
                table: "Photoes",
                column: "EmployeeId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Photoes_Employees_EmployeeId",
                table: "Photoes",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
