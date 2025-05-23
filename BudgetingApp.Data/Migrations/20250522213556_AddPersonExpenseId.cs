using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetingApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonExpenseId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PersonExpenses",
                table: "PersonExpenses");

            migrationBuilder.AddColumn<int>(
                name: "PersonExpenseId",
                table: "PersonExpenses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PersonExpenses",
                table: "PersonExpenses",
                column: "PersonExpenseId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonExpenses_PersonId_ExpenseId",
                table: "PersonExpenses",
                columns: new[] { "PersonId", "ExpenseId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PersonExpenses",
                table: "PersonExpenses");

            migrationBuilder.DropIndex(
                name: "IX_PersonExpenses_PersonId_ExpenseId",
                table: "PersonExpenses");

            migrationBuilder.DropColumn(
                name: "PersonExpenseId",
                table: "PersonExpenses");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PersonExpenses",
                table: "PersonExpenses",
                columns: new[] { "PersonId", "ExpenseId" });
        }
    }
}
