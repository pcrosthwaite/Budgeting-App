using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetingApp.Web.Shared
{
    public sealed class AlertModel
    {
        public Severity Severity { get; set; } = Severity.Info; // MudBlazor enum
        public string Message { get; set; }
    }
}