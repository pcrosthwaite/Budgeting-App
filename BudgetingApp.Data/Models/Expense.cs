namespace BudgetingApp.Data.Models
{
    public class Expense
    {
        public int ExpenseId { get; set; } // Primary Key
        public string Name { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public ExpenseFrequency Frequency { get; set; }
        public bool IncludeInBillsAccount { get; set; }

        // Navigation property for the many-to-many relationship
        public List<PersonExpense> PersonExpenses { get; set; } = new();

        public decimal FortnightlyCost => Frequency switch
        {
            ExpenseFrequency.Weekly => Cost * 2,
            ExpenseFrequency.Fortnightly => Cost,
            ExpenseFrequency.Monthly => Cost / 2,
            ExpenseFrequency.Quarterly => Cost / 6, // Quarterly divided into 6 fortnights
            ExpenseFrequency.SemiAnnually => Cost / 13, // SemiAnnually divided into 13 fortnights
            ExpenseFrequency.Yearly => Cost / 26, // Yearly divided into 26 fortnights
            _ => 0
        };
    }

    public enum ExpenseFrequency
    {
        Weekly,
        Fortnightly,
        Monthly,
        Quarterly, // Every 3 months
        Yearly, // Every year
        SemiAnnually
    }
}