namespace BudgetingApp.Data.Models
{
    public class Expense
    {
        public int ExpenseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public TransactionFrequency Frequency { get; set; }
        public bool IncludeInBillsAccount { get; set; }
        public bool IsSubscription { get; set; }

        public List<PersonExpense> PersonExpenses { get; set; } = new();

        public int? CategoryId { get; set; }
        public Category Category { get; set; }

        public string Notes { get; set; } = string.Empty;

        public decimal FortnightlyCost => Frequency switch
        {
            TransactionFrequency.Weekly => Cost * 2,
            TransactionFrequency.Fortnightly => Cost,
            TransactionFrequency.Monthly => Cost / 2,
            TransactionFrequency.Quarterly => Cost / 6, // Quarterly divided into 6 fortnights
            TransactionFrequency.SemiAnnually => Cost / 13, // SemiAnnually divided into 13 fortnights
            TransactionFrequency.Yearly => Cost / 26, // Yearly divided into 26 fortnights
            TransactionFrequency.FiveYearly => (Cost / 5) / 26, // Yearly divided into 26 fortnights
            _ => 0
        };
    }
}