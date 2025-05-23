namespace BudgetingApp.Data.Models
{
    public class PersonExpense
    {
        public int PersonExpenseId { get; set; } // Foreign Key
        public int PersonId { get; set; } // Foreign Key
        public Person Person { get; set; } = null!; // Navigation Property

        public int ExpenseId { get; set; } // Foreign Key
        public Expense Expense { get; set; } = null!; // Navigation Property

        public double Percentage { get; set; } // Added to store the percentage responsibility
    }
}