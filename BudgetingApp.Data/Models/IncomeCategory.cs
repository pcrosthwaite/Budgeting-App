namespace BudgetingApp.Data.Models
{
    public class IncomeCategory : Category
    {
        public IEnumerable<Income> Income { get; set; }

        public IncomeCategory()
        {
            CategoryType = CategoryType.Income;
        }
    }
}