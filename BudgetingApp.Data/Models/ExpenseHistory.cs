using System.ComponentModel.DataAnnotations;

namespace BudgetingApp.Data.Models
{
    public class ExpenseHistory : ISoftDelete
    {
        public int ExpenseHistoryId { get; set; }

        public int ExpenseId { get; set; }
        public Expense Expense { get; set; }

        public decimal Cost { get; set; }

        public DateTime ChangedDate { get; set; }
        public string Notes { get; set; }
    }
}