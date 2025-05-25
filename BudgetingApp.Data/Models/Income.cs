namespace BudgetingApp.Data.Models
{
    public class Income
    {
        public int IncomeId { get; set; }
        public int? PersonId { get; set; }
        public Person Person { get; set; }

        public decimal Amount { get; set; }
        public TransactionFrequency Frequency { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}