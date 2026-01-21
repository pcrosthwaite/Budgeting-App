using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetingApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNameToBankAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "BankAccounts",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "BankAccounts");
        }
    }
}
