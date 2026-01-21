using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetingApp.Data.Models
{
    public class Bank : ISoftDelete
    {
        public int BankId { get; set; }
        public string Name { get; set; }
    }
}