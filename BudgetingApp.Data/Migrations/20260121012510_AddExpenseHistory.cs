using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetingApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddExpenseHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncludeInBillsAccount",
                table: "Expenses");

            migrationBuilder.CreateTable(
                name: "ExpenseHistories",
                columns: table => new
                {
                    ExpenseHistoryId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ExpenseId = table.Column<int>(type: "INTEGER", nullable: false),
                    Cost = table.Column<double>(type: "REAL", nullable: false),
                    ChangedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseHistories", x => x.ExpenseHistoryId);
                    table.ForeignKey(
                        name: "FK_ExpenseHistories_Expenses_ExpenseId",
                        column: x => x.ExpenseId,
                        principalTable: "Expenses",
                        principalColumn: "ExpenseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseHistories_ExpenseId_ChangedDate",
                table: "ExpenseHistories",
                columns: new[] { "ExpenseId", "ChangedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseHistories_IsDeleted",
                table: "ExpenseHistories",
                column: "IsDeleted",
                filter: "IsDeleted = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpenseHistories");

            migrationBuilder.AddColumn<bool>(
                name: "IncludeInBillsAccount",
                table: "Expenses",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
