using System.ComponentModel;

namespace BudgetingApp.Data.Models
{
    public enum PaymentFrequency
    {
        Weekly,
        Fortnightly,
        Monthly,
    }

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