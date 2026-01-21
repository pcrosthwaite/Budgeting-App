using BudgetingApp.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetingApp.Web.Features.ExpenseHistory
{
    public class IndexQuery : IRequest<IndexQuery>
    {
        public int Years { get; set; } = 1; // Default to 1 year
        public List<ExpenseHistoryData> Expenses { get; set; } = new();
    }

    public class ExpenseHistoryData
    {
        public int ExpenseId { get; set; }
        public string ExpenseName { get; set; } = string.Empty;
        public List<ExpenseHistoryPoint> HistoryPoints { get; set; } = new();
    }

    public class ExpenseHistoryPoint
    {
        public DateTime Date { get; set; }
        public decimal Cost { get; set; }
        public string Notes { get; set; }
    }

    public class ExpenseHistoryQueryHandler : IRequestHandler<IndexQuery, IndexQuery>
    {
        private readonly BudgetingDbContext _context;

        // Toggle this to enable/disable mock data
        private const bool UseDummyData = false;

        public ExpenseHistoryQueryHandler(BudgetingDbContext context)
        {
            _context = context;
        }

        public async Task<IndexQuery> Handle(IndexQuery request, CancellationToken cancellationToken)
        {
            var data = new List<ExpenseHistoryData>();

            if (UseDummyData)
            {
                data = await GetMockData(request, cancellationToken);
            }
            else
            {
                data = await GetRealData(request, cancellationToken);
            }

            request.Expenses = data;

            return request;
        }

        private async Task<List<ExpenseHistoryData>> GetRealData(IndexQuery request, CancellationToken cancellationToken)
        {
            var startDate = DateTime.UtcNow.AddYears(-request.Years);

            // Get all expenses that have history within the date range
            var expensesWithHistory = await _context.Expenses
                .Where(e => e.ExpenseHistory.Any(x => x.ChangedDate >= startDate))
                .Select(e => new ExpenseHistoryData
                {
                    ExpenseId = e.ExpenseId,
                    ExpenseName = e.Name,
                    HistoryPoints = e.ExpenseHistory
                        .Where(x => x.ChangedDate >= startDate)
                        .OrderBy(x => x.ChangedDate)
                        .Select(x => new ExpenseHistoryPoint
                        {
                            Date = x.ChangedDate,
                            Cost = x.Cost,
                            Notes = x.Notes
                        })
                        .ToList()
                })
                .ToListAsync(cancellationToken);

            return expensesWithHistory;
        }

        private async Task<List<ExpenseHistoryData>> GetMockData(IndexQuery request, CancellationToken cancellationToken)
        {
            // Get all expenses for mock data generation
            var expenses = await _context.Expenses
                .Select(e => new { e.ExpenseId, e.Name, e.Cost })
                .ToListAsync(cancellationToken);

            var mockExpenses = new List<ExpenseHistoryData>();
            var random = new Random(42); // Fixed seed for consistent mock data

            foreach (var expense in expenses)
            {
                var historyPoints = GenerateMockHistoryPoints(
                    expense.Cost,
                    request.Years,
                    expense.Name,
                    random
                );

                mockExpenses.Add(new ExpenseHistoryData
                {
                    ExpenseId = expense.ExpenseId,
                    ExpenseName = expense.Name,
                    HistoryPoints = historyPoints
                });
            }

            return mockExpenses;
        }

        private List<ExpenseHistoryPoint> GenerateMockHistoryPoints(decimal baseCost, int years, string expenseName, Random random)
        {
            var points = new List<ExpenseHistoryPoint>();
            var currentDate = DateTime.UtcNow.AddYears(-years);
            var endDate = DateTime.UtcNow;
            var currentCost = baseCost;

            // Generate monthly data points
            while (currentDate <= endDate)
            {
                // Add some realistic variation to the cost
                var variation = GenerateCostVariation(expenseName, random, currentCost);
                currentCost += variation;

                // Ensure cost doesn't go negative
                if (currentCost < 0) currentCost = baseCost * 0.1m;

                points.Add(new ExpenseHistoryPoint
                {
                    Date = currentDate,
                    Cost = Math.Round(currentCost, 2),
                    Notes = GenerateMockNotes(variation, expenseName, random)
                });

                // Move to first day of next month
                currentDate = currentDate.AddMonths(1);
                if (currentDate.Day != 1)
                {
                    currentDate = new DateTime(currentDate.Year, currentDate.Month, 1);
                }
            }

            return points;
        }

        private decimal GenerateCostVariation(string expenseName, Random random, decimal currentCost)
        {
            // Different variation patterns based on expense type
            var variationPercentage = expenseName.ToLower() switch
            {
                var name when name.Contains("rent") || name.Contains("mortgage") =>
                    // Housing costs: small increases, occasional larger jumps
                    random.NextDouble() < 0.1 ? (decimal)(random.NextDouble() * 0.1 + 0.02) :
                    (decimal)(random.NextDouble() * 0.02 - 0.01),

                var name when name.Contains("utility") || name.Contains("electric") || name.Contains("gas") =>
                    // Utilities: seasonal variation, more volatile
                    (decimal)(random.NextDouble() * 0.3 - 0.15),

                var name when name.Contains("insurance") =>
                    // Insurance: annual increases, mostly stable
                    random.NextDouble() < 0.08 ? (decimal)(random.NextDouble() * 0.1 + 0.03) : 0,

                var name when name.Contains("subscription") || name.Contains("netflix") || name.Contains("spotify") =>
                    // Subscriptions: occasional price increases
                    random.NextDouble() < 0.05 ? (decimal)(random.NextDouble() * 0.2 + 0.1) : 0,

                var name when name.Contains("grocery") || name.Contains("food") =>
                    // Food: inflation trends, moderate variation
                    (decimal)(random.NextDouble() * 0.1 - 0.03),

                _ =>
                    // General expenses: moderate variation
                    (decimal)(random.NextDouble() * 0.15 - 0.075)
            };

            return currentCost * variationPercentage;
        }

        private string GenerateMockNotes(decimal variation, string expenseName, Random random)
        {
            if (Math.Abs(variation) < 0.01m) return "No change";

            var isIncrease = variation > 0;
            var percentChange = Math.Abs(variation);

            var reasons = isIncrease ? new[]
            {
                "Price increase due to inflation",
                "Annual rate adjustment",
                "Service upgrade",
                "Market rate increase",
                "Contract renewal with higher rate",
                "Additional fees added",
                "Seasonal price adjustment"
            } : new[]
            {
                "Promotional discount applied",
                "Negotiated better rate",
                "Market competition reduction",
                "Loyalty discount",
                "Seasonal adjustment",
                "Service downgrade",
                "Bundle savings"
            };

            return reasons[random.Next(reasons.Length)];
        }
    }
}