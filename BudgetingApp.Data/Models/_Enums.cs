using System.ComponentModel;

namespace BudgetingApp.Data.Models
{
    public enum TransactionFrequency
    {
        Weekly,
        Fortnightly,
        Monthly,
        Quarterly, // Every 3 months
        Yearly, // Every year

        [Description("Semi-Annually")]
        SemiAnnually,

        [Description("Five-Yearly")]
        FiveYearly
    }
}