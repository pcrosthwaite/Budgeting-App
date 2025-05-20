namespace BudgetingApp.Data.Models
{
    public class Person : ISoftDelete
    {
        public int PersonId { get; set; } // Primary Key
        public string Name { get; set; } = string.Empty;

        // Navigation property for the many-to-many relationship
        public List<PersonExpense> PersonExpenses { get; set; } = new();
    }
}