namespace BudgetingApp.Data.Models
{
    public class Person : ISoftDelete
    {
        public int PersonId { get; set; } // Primary Key
        public string Name { get; set; } = string.Empty;

        public int? IncomeId { get; set; }
        public Income Income { get; set; }

        // Navigation property for the many-to-many relationship
        public List<PersonExpense> PersonExpenses { get; set; } = new();
    }
}