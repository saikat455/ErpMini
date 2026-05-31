using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ErpMini.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyMultiTenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "erp",
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "Departments",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "Departments",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "Designations",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "Designations",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "Designations",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                schema: "erp",
                table: "Vendors",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                schema: "erp",
                table: "users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                schema: "erp",
                table: "users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                schema: "erp",
                table: "Transactions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                schema: "erp",
                table: "PurchaseOrders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                schema: "erp",
                table: "Employees",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                schema: "erp",
                table: "Designations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                schema: "erp",
                table: "Departments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Companies",
                schema: "erp",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CompanyCode = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "LeaveTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "LeaveTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "LeaveTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "Description",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_CompanyId",
                schema: "erp",
                table: "Vendors",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CompanyId",
                schema: "erp",
                table: "Transactions",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_CompanyId",
                schema: "erp",
                table: "PurchaseOrders",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CompanyId",
                schema: "erp",
                table: "Employees",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Designations_CompanyId",
                schema: "erp",
                table: "Designations",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_CompanyId",
                schema: "erp",
                table: "Departments",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Companies_CompanyId",
                schema: "erp",
                table: "Departments",
                column: "CompanyId",
                principalSchema: "erp",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Designations_Companies_CompanyId",
                schema: "erp",
                table: "Designations",
                column: "CompanyId",
                principalSchema: "erp",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Companies_CompanyId",
                schema: "erp",
                table: "Employees",
                column: "CompanyId",
                principalSchema: "erp",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_Companies_CompanyId",
                schema: "erp",
                table: "PurchaseOrders",
                column: "CompanyId",
                principalSchema: "erp",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Companies_CompanyId",
                schema: "erp",
                table: "Transactions",
                column: "CompanyId",
                principalSchema: "erp",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vendors_Companies_CompanyId",
                schema: "erp",
                table: "Vendors",
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
                name: "FK_Departments_Companies_CompanyId",
                schema: "erp",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Designations_Companies_CompanyId",
                schema: "erp",
                table: "Designations");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Companies_CompanyId",
                schema: "erp",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_Companies_CompanyId",
                schema: "erp",
                table: "PurchaseOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Companies_CompanyId",
                schema: "erp",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_Companies_CompanyId",
                schema: "erp",
                table: "Vendors");

            migrationBuilder.DropTable(
                name: "Companies",
                schema: "erp");

            migrationBuilder.DropIndex(
                name: "IX_Vendors_CompanyId",
                schema: "erp",
                table: "Vendors");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_CompanyId",
                schema: "erp",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrders_CompanyId",
                schema: "erp",
                table: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_Employees_CompanyId",
                schema: "erp",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Designations_CompanyId",
                schema: "erp",
                table: "Designations");

            migrationBuilder.DropIndex(
                name: "IX_Departments_CompanyId",
                schema: "erp",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "erp",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "erp",
                table: "users");

            migrationBuilder.DropColumn(
                name: "Role",
                schema: "erp",
                table: "users");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "erp",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "erp",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "erp",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "erp",
                table: "Designations");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "erp",
                table: "Departments");

            migrationBuilder.InsertData(
                schema: "erp",
                table: "Departments",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "IsActive", "IsDeleted", "Name", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "HR Department", true, false, "Human Resources", null, null },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "IT Department", true, false, "Information Technology", null, null },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Finance Department", true, false, "Finance", null, null }
                });

            migrationBuilder.InsertData(
                schema: "erp",
                table: "Designations",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "IsActive", "IsDeleted", "Title", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "Software Engineer", null, null },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "Senior Engineer", null, null },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "HR Manager", null, null },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "Accountant", null, null }
                });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "LeaveTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: "Yearly paid leave");

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "LeaveTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "Description",
                value: "Medical leave");

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "LeaveTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "Description",
                value: "Short personal leave");
        }
    }
}
