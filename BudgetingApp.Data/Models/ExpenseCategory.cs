namespace BudgetingApp.Data.Models
{
    public class ExpenseCategory : Category
    {
        public IEnumerable<Expense> Expenses { get; set; }

        public ExpenseCategory()
        {
            CategoryType = CategoryType.Expense;
        }
    }
}