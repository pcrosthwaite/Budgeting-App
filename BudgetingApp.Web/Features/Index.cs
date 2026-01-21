using ApexCharts;
using BudgetingApp.Data;
using BudgetingApp.Data.Models;
using BudgetingApp.Data.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using System.Globalization;
using static BudgetingApp.Web.Features.IndexQueryHandler;

namespace BudgetingApp.Web.Features
{
    public class IndexQuery : IRequest<IndexQuery>
    {
        public List<IndexModel> Expenses { get; set; } = new();
        public List<IndexModel> BillTransfers => Expenses.Where(e => e.BankAccountId != null).ToList();
        public List<IndexModel> Subscriptions => Expenses.Where(e => e.IsSubscription).ToList();

        // Lookup dictionary for person shares
        public List<PersonModel> TotalPersons { get; set; } = new();

        //public List<BillTransferModel> BillTransferOverview { get; set; }
        public BillTransferPivotModel BillTransferOverview { get; set; }

        public List<PersonOverviewModel> PersonOverview { get; set; } = new();
        public ApexChartOptions<ChartData> BillChartOptions { get; set; }
        public ApexChartOptions<ChartData> SubscriptionChartOptions { get; set; }

        public List<ChartData> BillOverviewData = new();
        public List<ChartData> SubscriptionOverviewData = new();
    }

    /// <summary>
    /// <see cref="Expense"/>
    /// </summary>
    public class IndexModel
    {
        public int ExpenseId { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public TransactionFrequency Frequency { get; set; }
        public string CategoryName { get; set; }
        public int? BankAccountId { get; set; }
        public BankAccount BankAccount { get; set; }
        public string BankAccountName => BankAccount?.Name ?? string.Empty;
        public bool IsSubscription { get; set; }

        public List<PersonExpenseModel> PersonExpenses { get; set; }

        public decimal FortnightlyCost { get; set; }
    }

    public class PersonModel
    {
        public int PersonId { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// <see cref="PersonExpense"/>
    /// </summary>
    public class PersonExpenseModel
    {
        public int PersonExpenseId { get; set; }
        public int PersonId { get; set; }
        public string PersonName { get; set; }

        public int ExpenseId { get; set; }
        public string ExpenseName { get; set; }

        public double Percentage { get; set; }
    }

    public class BillTransferModel
    {
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        public List<BillTransferAccountModel> Accounts { get; set; } = new();
    }

    public sealed class BillTransferAccountModel
    {
        public int BankAccountId { get; set; }
        public string BankAccountName { get; set; } = "Unknown";
        public decimal Share { get; set; }
    }

    public class PersonOverviewModel
    {
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        public decimal Income { get; set; }
        public decimal TotalOutgoings { get; set; }
        public decimal OutgoingPercent => Income > 0 ? (TotalOutgoings / Income) * 100 : 0;
        public string OutgoingDisplay => $"{TotalOutgoings.ToString("C2", CultureInfo.GetCultureInfo("en-AU"))} ({Math.Round(OutgoingPercent, 2)}%)";
        public decimal? Remaining => Income > 0 && Income > TotalOutgoings ? Income - TotalOutgoings : null;
    }

    public class IndexQueryHandler : IRequestHandler<IndexQuery, IndexQuery>
    {
        private readonly ExpenseService _expenseService;
        private readonly IMapperService _mapper;
        private readonly BudgetingDbContext _context;

        public IndexQueryHandler(ExpenseService expenseService, IMapperService mapper, BudgetingDbContext context)
        {
            _expenseService = expenseService;
            _context = context;
            _mapper = mapper;
        }

        public async Task<IndexQuery> Handle(IndexQuery request, CancellationToken cancellationToken)
        {
            var expenses = await _expenseService.GetExpensesAsync();

            request.Expenses = _mapper.MapList<Expense, IndexModel>(expenses).ToList();

            var totalPeople = request.Expenses.SelectMany(e => e.PersonExpenses).GroupBy(x => x.PersonId).Select(g => g.FirstOrDefault()).ToList().Select(x => new PersonModel() { PersonId = x.PersonId, Name = x.PersonName });

            request.TotalPersons = totalPeople.ToList();

            var bankAccounts = await _context.BankAccounts.ToListAsync(cancellationToken);
            request.BillTransferOverview = GetTransferOverview(request.BillTransfers, bankAccounts);

            //var billTotals = request.BillTransfers
            //                    .Where(e => e.BankAccountId != null)
            //                    .SelectMany(
            //                                e => e.PersonExpenses ?? Enumerable.Empty<PersonExpenseModel>(),
            //                                (e, pe) => new
            //                                {
            //                                    pe.PersonId,
            //                                    pe.PersonName,
            //                                    BankAccountId = e.BankAccountId!.Value,
            //                                    BankAccountName = e.BankAccount?.Name ?? "Unknown",
            //                                    Share = e.FortnightlyCost * (decimal)pe.Percentage
            //                                })
            //                    .GroupBy(x => new { x.PersonId, x.PersonName, x.BankAccountId, x.BankAccountName })
            //                    .Select(g => new
            //                    {
            //                        g.Key.PersonId,
            //                        g.Key.PersonName,
            //                        g.Key.BankAccountId,
            //                        g.Key.BankAccountName,
            //                        Share = g.Sum(x => x.Share)
            //                    })
            //                    .GroupBy(x => new { x.PersonId, x.PersonName })
            //                    .Select(g => new BillTransferModel()
            //                    {
            //                        PersonId = g.Key.PersonId,
            //                        PersonName = g.Key.PersonName,
            //                        Accounts = g.OrderBy(x => x.BankAccountName)
            //                                    .Select(x => new BillTransferAccountModel()
            //                                    {
            //                                        BankAccountId = x.BankAccountId,
            //                                        BankAccountName = x.BankAccountName,
            //                                        Share = x.Share
            //                                    })
            //                                    .ToList()
            //                    })
            //                    .OrderBy(x => x.PersonName)
            //                    .ToList();

            //request.BillTransferOverview = billTotals;

            var incomes = await _context.Income
                                    .Include(x => x.Person)
                                    .ToListAsync(cancellationToken);

            var totalOutgoingsByPerson = request.Expenses
                                        .SelectMany(e => e.PersonExpenses.Select(pe => new { pe.PersonId, Share = e.FortnightlyCost * (decimal)pe.Percentage }))
                                        .GroupBy(x => x.PersonId)
                                        .ToDictionary(g => g.Key, g => g.Sum(x => x.Share));

            foreach (var income in incomes)
            {
                var personOverview = new PersonOverviewModel()
                {
                    PersonId = income.PersonId ?? 0,
                    PersonName = income.Person?.Name ?? "Unknown",
                    Income = income.Amount,
                    TotalOutgoings = totalOutgoingsByPerson.TryGetValue(income.PersonId ?? 0, out var totalOutgoings) ? totalOutgoings : 0
                };

                request.PersonOverview.Add(personOverview);
            }
            // 1) Group by category and sum cost (swap to FortnightlyCost if you prefer)
            var totals = request.Expenses.GroupBy(e => e.CategoryName).Select(g => new { Category = g.Key, Sum = g.Sum(e => e.Cost) }).ToList();

            // 2) Extract labels and data
            //request.BillOverviewLabels = request.Expenses.GroupBy(e => e.CategoryName).Select(g => g.Key).ToArray();

            //request.BillOverviewData = request.Expenses.GroupBy(e => e.CategoryName).Select(g => (double)g.Sum(e => e.Cost)).ToArray();

            var billDataOverview = request.Expenses.GroupBy(e => e.CategoryName).Select(g => new ChartData { Name = g.Key, Total = g.Sum(e => e.FortnightlyCost) }).ToList();
            var subscriptionOverview = request.Subscriptions.GroupBy(e => e.Name).Select(g => new ChartData { Name = g.Key, Total = g.Sum(e => e.FortnightlyCost) }).ToList();
            request.BillOverviewData = billDataOverview;
            request.SubscriptionOverviewData = subscriptionOverview;

            request.BillChartOptions = new ApexChartOptions<ChartData>
            {
                Chart = new Chart { Id = "bill-chart" },
                Tooltip = new Tooltip
                {
                    Y = new TooltipY
                    {
                        Formatter = @"
                        function (val) {
                          return new Intl.NumberFormat('en-AU', {
                            style: 'currency',
                            currency: 'AUD'
                          }).format(val);
                        }
                    "
                    }
                }
            };

            request.SubscriptionChartOptions = new ApexChartOptions<ChartData>
            {
                Chart = new Chart { Id = "subs-chart" },
                Tooltip = request.BillChartOptions.Tooltip
            };

            return request;
        }

        public sealed class BillTransferPivotRow
        {
            public int PersonId { get; set; }
            public string PersonName { get; set; } = string.Empty;

            // BankAccountId -> Amount
            public Dictionary<int, decimal> AmountByAccountId { get; set; } = new();

            public decimal Total => AmountByAccountId.Values.Sum();
        }

        public sealed class BillTransferPivotModel
        {
            public List<BankAccountColumn> Columns { get; set; } = new();
            public List<BillTransferPivotRow> Rows { get; set; } = new();
        }

        private BillTransferPivotModel GetTransferOverview(List<IndexModel> billTransfers, List<BankAccount> bankAccounts)
        {
            var bankAccountLookup = bankAccounts.ToDictionary(a => a.BankAccountId, a => a.Name);

            var raw = billTransfers
                        .Where(e => e.BankAccountId != null)
                        .SelectMany(e => e.PersonExpenses ?? Enumerable.Empty<PersonExpenseModel>(),
                        (e, pe) => new
                        {
                            pe.PersonId,
                            pe.PersonName,
                            BankAccountId = e.BankAccountId!.Value,
                            Share = e.FortnightlyCost * (decimal)pe.Percentage
                        }).ToList();

            var columns = raw
                .Select(x => x.BankAccountId)
                .Distinct()
                .OrderBy(id => bankAccountLookup.TryGetValue(id, out var name) ? name : "Unknown")
                .Select(id => new BankAccountColumn
                {
                    BankAccountId = id,
                    BankAccountName = bankAccountLookup.TryGetValue(id, out var name) ? name : "Unknown"
                }).ToList();

            var rows = raw
                        .GroupBy(x => new { x.PersonId, x.PersonName })
                        .Select(g =>
                        {
                            var row = new BillTransferPivotRow
                            {
                                PersonId = g.Key.PersonId,
                                PersonName = g.Key.PersonName
                            };

                            row.AmountByAccountId = g
                                .GroupBy(x => x.BankAccountId)
                                .ToDictionary(gg => gg.Key, gg => gg.Sum(v => v.Share));

                            return row;
                        })
                        .OrderBy(r => r.PersonName)
                        .ToList();

            var result = new BillTransferPivotModel
            {
                Columns = columns,
                Rows = rows
            };

            return result;
        }
    }

    public class BankAccountColumn
    {
        public int BankAccountId { get; set; }
        public string BankAccountName { get; set; } = string.Empty;
    }

    public class ChartData
    {
        public string Name { get; set; } = "";
        public decimal Total { get; set; }
    }
}