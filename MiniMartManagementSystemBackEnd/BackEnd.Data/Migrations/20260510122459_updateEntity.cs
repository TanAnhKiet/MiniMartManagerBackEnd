using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                table: "Imports",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Imports_EmployeeId",
                table: "Imports",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Imports_Employee_EmployeeId",
                table: "Imports",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Imports_Employee_EmployeeId",
                table: "Imports");

            migrationBuilder.DropIndex(
                name: "IX_Imports_EmployeeId",
                table: "Imports");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Imports");
        }
    }
}
