using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ErpMini.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixCompanyIdColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Companies_CompanyId1",
                schema: "erp",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Designations_Companies_CompanyId1",
                schema: "erp",
                table: "Designations");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Companies_CompanyId1",
                schema: "erp",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_CompanyId1",
                schema: "erp",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Designations_CompanyId1",
                schema: "erp",
                table: "Designations");

            migrationBuilder.DropIndex(
                name: "IX_Departments_CompanyId1",
                schema: "erp",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "CompanyId1",
                schema: "erp",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "CompanyId1",
                schema: "erp",
                table: "Designations");

            migrationBuilder.DropColumn(
                name: "CompanyId1",
                schema: "erp",
                table: "Departments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyId1",
                schema: "erp",
                table: "Employees",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId1",
                schema: "erp",
                table: "Designations",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId1",
                schema: "erp",
                table: "Departments",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CompanyId1",
                schema: "erp",
                table: "Employees",
                column: "CompanyId1");

            migrationBuilder.CreateIndex(
                name: "IX_Designations_CompanyId1",
                schema: "erp",
                table: "Designations",
                column: "CompanyId1");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_CompanyId1",
                schema: "erp",
                table: "Departments",
                column: "CompanyId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Companies_CompanyId1",
                schema: "erp",
                table: "Departments",
                column: "CompanyId1",
                principalSchema: "erp",
                principalTable: "Companies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Designations_Companies_CompanyId1",
                schema: "erp",
                table: "Designations",
                column: "CompanyId1",
                principalSchema: "erp",
                principalTable: "Companies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Companies_CompanyId1",
                schema: "erp",
                table: "Employees",
                column: "CompanyId1",
                principalSchema: "erp",
                principalTable: "Companies",
                principalColumn: "Id");
        }
    }
}
