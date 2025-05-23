namespace BudgetingApp.Data.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string HexColourCode { get; set; }

        public CategoryType CategoryType { get; set; }
    }

    public enum CategoryType
    {
        Expense = 1,
        Income = 2
    }
}