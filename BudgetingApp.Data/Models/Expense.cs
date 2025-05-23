namespace BudgetingApp.Data.Models
{
    public class Expense
    {
        public int ExpenseId { get; set; } // Primary Key
        public string Name { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public TransactionFrequency Frequency { get; set; }
        public bool IncludeInBillsAccount { get; set; }

        // Navigation property for the many-to-many relationship
        public List<PersonExpense> PersonExpenses { get; set; } = new();

        public int? CategoryId { get; set; } // Foreign Key
        public ExpenseCategory ExpenseCategory { get; set; } // Navigation Property

        public decimal FortnightlyCost => Frequency switch
        {
            TransactionFrequency.Weekly => Cost * 2,
            TransactionFrequency.Fortnightly => Cost,
            TransactionFrequency.Monthly => Cost / 2,
            TransactionFrequency.Quarterly => Cost / 6, // Quarterly divided into 6 fortnights
            TransactionFrequency.SemiAnnually => Cost / 13, // SemiAnnually divided into 13 fortnights
            TransactionFrequency.Yearly => Cost / 26, // Yearly divided into 26 fortnights
            _ => 0
        };
    }
}