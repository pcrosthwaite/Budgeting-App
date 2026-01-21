using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetingApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBankAccountToExpense : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_Banks_BankId",
                table: "BankAccounts");

            migrationBuilder.AddColumn<int>(
                name: "BankAccountId",
                table: "Expenses",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_BankAccountId",
                table: "Expenses",
                column: "BankAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_Banks_BankId",
                table: "BankAccounts",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "BankId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_BankAccounts_BankAccountId",
                table: "Expenses",
                column: "BankAccountId",
                principalTable: "BankAccounts",
                principalColumn: "BankAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_Banks_BankId",
                table: "BankAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_BankAccounts_BankAccountId",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_BankAccountId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "BankAccountId",
                table: "Expenses");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_Banks_BankId",
                table: "BankAccounts",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "BankId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
