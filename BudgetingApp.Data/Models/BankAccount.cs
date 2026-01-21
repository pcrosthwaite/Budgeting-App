using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetingApp.Data.Models
{
    public class BankAccount : ISoftDelete
    {
        public int BankAccountId { get; set; }
        public string Name { get; set; }
        public string AccountNumber { get; set; }
        public int BankId { get; set; }
        public Bank Bank { get; set; }
    }
}