using BudgetingApp.Data.Models;
using BudgetingApp.Web.Shared;
using MediatR;
using MudBlazor;

namespace BudgetingApp.Web.Features.Tools.LoanRepayments
{
    public class IndexQuery : IRequest<IndexModel>
    {
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime TargetDate { get; set; }
        public PaymentFrequency Frequency { get; set; } = PaymentFrequency.Fortnightly;
    }

    public class IndexModel
    {
        public decimal OriginalAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime TargetDate { get; set; }
        public PaymentFrequency Frequency { get; set; }

        public decimal SuggestedRegularPayment { get; set; }
        public List<PaymentRow> Rows { get; set; } = new();

        public List<AlertModel> Alerts { get; set; } = new();
    }

    public class PaymentRow
    {
        public int PaymentNumber { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal BalanceBefore { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal BalanceAfter { get; set; }
    }

    public class IndexHandler : IRequestHandler<IndexQuery, IndexModel>
    {
        public async Task<IndexModel> Handle(IndexQuery request, CancellationToken cancellationToken)
        {
            var model = new IndexModel();
            if (request.Amount <= 0)
            {
                model.Alerts.Add(new AlertModel { Severity = Severity.Error, Message = "Amount must be greater than zero." });

                return model;
            }

            if (request.TargetDate < request.StartDate)
            {
                model.Alerts.Add(new AlertModel { Severity = Severity.Error, Message = "Target date must be on or after the start date." });

                return model;
            }

            var paymentDates = BuildPaymentDates(request.StartDate, request.TargetDate, request.Frequency);

            if (paymentDates.Count == 0)
            {
                model.Alerts.Add(new AlertModel { Severity = Severity.Error, Message = "No payments could be scheduled for the selected dates/frequency." });

                return model;
            }

            // regular payment, final payment clears remaining
            var regularPayment = decimal.Round(request.Amount / paymentDates.Count, 2, MidpointRounding.AwayFromZero);
            model.SuggestedRegularPayment = regularPayment;

            if (paymentDates[^1].Date < request.TargetDate.Date)
            {
                model.Alerts.Add(new AlertModel
                {
                    Severity = Severity.Warning,
                    Message = $"Final scheduled payment ({paymentDates[^1]:d}) is before target date ({request.TargetDate:d})."
                });
            }

            var remaining = request.Amount;

            for (var i = 0; i < paymentDates.Count; i++)
            {
                var before = remaining;

                var payment =
                    (i == paymentDates.Count - 1)
                        ? before // last payment clears remainder exactly
                        : regularPayment;

                payment = Decimal.Round(payment, 2, MidpointRounding.AwayFromZero);

                var after = before - payment;

                if (after < 0)
                {
                    // safety clamp in case rounding ever pushes over
                    payment = before;
                    after = 0;
                }

                model.Rows.Add(new PaymentRow
                {
                    PaymentNumber = i + 1,
                    PaymentDate = paymentDates[i],
                    BalanceBefore = before,
                    PaymentAmount = payment,
                    BalanceAfter = after
                });

                remaining = after;
            }

            return model;
        }

        private static List<DateTime> BuildPaymentDates(DateTime start, DateTime target, PaymentFrequency frequency)
        {
            var dates = new List<DateTime>();

            var d = start.Date;

            while (d <= target.Date)
            {
                dates.Add(d);
                d = AddPeriod(d, frequency);
            }

            return dates;
        }

        private static DateTime AddPeriod(DateTime date, PaymentFrequency frequency)
        {
            return frequency switch
            {
                PaymentFrequency.Weekly => date.AddDays(7),
                PaymentFrequency.Fortnightly => date.AddDays(14),
                PaymentFrequency.Monthly => date.AddMonths(1),
                _ => date.AddDays(14)
            };
        }
    }
}