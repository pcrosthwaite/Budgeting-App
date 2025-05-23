using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetingApp.Data.Models
{
    public enum TransactionFrequency
    {
        Weekly,
        Fortnightly,
        Monthly,
        Quarterly, // Every 3 months
        Yearly, // Every year
        SemiAnnually
    }
}