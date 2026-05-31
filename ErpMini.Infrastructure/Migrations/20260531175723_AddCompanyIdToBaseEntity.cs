using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ErpMini.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyIdToBaseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "erp",
                table: "AccountCategories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "AccountCategories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "AccountCategories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "AccountCategories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "AccountCategories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "LeaveTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "LeaveTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "LeaveTypes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                schema: "erp",
                table: "PurchaseOrderItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                schema: "erp",
                table: "Payrolls",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                schema: "erp",
                table: "LeaveTypes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                schema: "erp",
                table: "LeaveApplications",
                type: "integer",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                schema: "erp",
                table: "Companies",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                schema: "erp",
                table: "AccountCategories",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItems_CompanyId",
                schema: "erp",
                table: "PurchaseOrderItems",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Payrolls_CompanyId",
                schema: "erp",
                table: "Payrolls",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveTypes_CompanyId",
                schema: "erp",
                table: "LeaveTypes",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveApplications_CompanyId",
                schema: "erp",
                table: "LeaveApplications",
                column: "CompanyId");

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

            migrationBuilder.CreateIndex(
                name: "IX_AccountCategories_CompanyId",
                schema: "erp",
                table: "AccountCategories",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountCategories_Companies_CompanyId",
                schema: "erp",
                table: "AccountCategories",
                column: "CompanyId",
                principalSchema: "erp",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveApplications_Companies_CompanyId",
                schema: "erp",
                table: "LeaveApplications",
                column: "CompanyId",
                principalSchema: "erp",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveTypes_Companies_CompanyId",
                schema: "erp",
                table: "LeaveTypes",
                column: "CompanyId",
                principalSchema: "erp",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payrolls_Companies_CompanyId",
                schema: "erp",
                table: "Payrolls",
                column: "CompanyId",
                principalSchema: "erp",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderItems_Companies_CompanyId",
                schema: "erp",
                table: "PurchaseOrderItems",
                column: "CompanyId",
                principalSchema: "erp",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountCategories_Companies_CompanyId",
                schema: "erp",
                table: "AccountCategories");

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

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveApplications_Companies_CompanyId",
                schema: "erp",
                table: "LeaveApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveTypes_Companies_CompanyId",
                schema: "erp",
                table: "LeaveTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_Payrolls_Companies_CompanyId",
                schema: "erp",
                table: "Payrolls");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderItems_Companies_CompanyId",
                schema: "erp",
                table: "PurchaseOrderItems");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrderItems_CompanyId",
                schema: "erp",
                table: "PurchaseOrderItems");

            migrationBuilder.DropIndex(
                name: "IX_Payrolls_CompanyId",
                schema: "erp",
                table: "Payrolls");

            migrationBuilder.DropIndex(
                name: "IX_LeaveTypes_CompanyId",
                schema: "erp",
                table: "LeaveTypes");

            migrationBuilder.DropIndex(
                name: "IX_LeaveApplications_CompanyId",
                schema: "erp",
                table: "LeaveApplications");

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

            migrationBuilder.DropIndex(
                name: "IX_AccountCategories_CompanyId",
                schema: "erp",
                table: "AccountCategories");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "erp",
                table: "PurchaseOrderItems");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "erp",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "erp",
                table: "LeaveTypes");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "erp",
                table: "LeaveApplications");

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

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "erp",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "erp",
                table: "AccountCategories");

            migrationBuilder.InsertData(
                schema: "erp",
                table: "AccountCategories",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "IsActive", "IsDeleted", "Name", "Type", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "Sales Revenue", "Income", null, null },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "Service Income", "Income", null, null },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "Office Rent", "Expense", null, null },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "Utilities", "Expense", null, null },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "Salaries", "Expense", null, null }
                });

            migrationBuilder.InsertData(
                schema: "erp",
                table: "LeaveTypes",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "IsActive", "IsDeleted", "Name", "TotalDays", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "Annual Leave", 20, null, null },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "Sick Leave", 14, null, null },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "Casual Leave", 10, null, null }
                });
        }
    }
}
