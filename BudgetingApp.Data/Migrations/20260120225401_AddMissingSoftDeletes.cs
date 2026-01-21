using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetingApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingSoftDeletes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Income",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Expenses",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Categories",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Income_IsDeleted",
                table: "Income",
                column: "IsDeleted",
                filter: "IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_IsDeleted",
                table: "Expenses",
                column: "IsDeleted",
                filter: "IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_IsDeleted",
                table: "Categories",
                column: "IsDeleted",
                filter: "IsDeleted = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Income_IsDeleted",
                table: "Income");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_IsDeleted",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Categories_IsDeleted",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Income");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Categories");
        }
    }
}
