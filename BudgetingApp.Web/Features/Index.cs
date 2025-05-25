using ApexCharts;
using BudgetingApp.Data.Models;
using BudgetingApp.Data.Services;
using MediatR;
using MudBlazor;

namespace BudgetingApp.Web.Features
{
    public class IndexQuery : IRequest<IndexQuery>
    {
        public List<IndexModel> Expenses { get; set; } = new();
        public List<IndexModel> BillTransfers => Expenses.Where(e => e.IncludeInBillsAccount).ToList();
        public List<IndexModel> Subscriptions => Expenses.Where(e => e.IsSubscription).ToList();

        // Lookup dictionary for person shares
        public List<PersonModel> TotalPersons { get; set; } = new();

        public List<BillTransferModel> BillTransferOverview { get; set; }
        public ApexChartOptions<CategoryData> BillOverviewChartOptions { get; internal set; }

        //public string[] BillOverviewLabels = Array.Empty<string>(); // new();
        public List<CategoryData> BillOverviewData = new(); // Array.Empty<double>(); //new();
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
        public bool IncludeInBillsAccount { get; set; }
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
        public decimal Share { get; set; }
    }

    public class IndexQueryHandler : IRequestHandler<IndexQuery, IndexQuery>
    {
        private readonly ExpenseService _expenseService;
        private readonly IMapperService _mapper;

        public IndexQueryHandler(ExpenseService expenseService, IMapperService mapper)
        {
            _expenseService = expenseService;
            _mapper = mapper;
        }

        public async Task<IndexQuery> Handle(IndexQuery request, CancellationToken cancellationToken)
        {
            var expenses = await _expenseService.GetExpensesAsync();

            request.Expenses = _mapper.MapList<Expense, IndexModel>(expenses).ToList();

            var totalPeople = request.Expenses.SelectMany(e => e.PersonExpenses).GroupBy(x => x.PersonId).Select(g => g.FirstOrDefault()).ToList().Select(x => new PersonModel() { PersonId = x.PersonId, Name = x.PersonName });

            request.TotalPersons = totalPeople.ToList();

            var billTotals = request.BillTransfers
                                .SelectMany(e => e.PersonExpenses.Select(pe => new
                                {
                                    pe.PersonId,
                                    pe.PersonName,
                                    Share = e.FortnightlyCost * (decimal)pe.Percentage
                                }))
                                .GroupBy(x => new { x.PersonId, x.PersonName })
                                .Select(g => new BillTransferModel()
                                {
                                    PersonId = g.Key.PersonId,
                                    PersonName = g.Key.PersonName,
                                    Share = g.Sum(x => x.Share)
                                })
                                .OrderBy(x => x.PersonName)
                                .ToList();

            request.BillTransferOverview = billTotals;

            // 1) Group by category and sum cost (swap to FortnightlyCost if you prefer)
            var totals = request.Expenses.GroupBy(e => e.CategoryName).Select(g => new { Category = g.Key, Sum = g.Sum(e => e.Cost) }).ToList();

            // 2) Extract labels and data
            //request.BillOverviewLabels = request.Expenses.GroupBy(e => e.CategoryName).Select(g => g.Key).ToArray();

            //request.BillOverviewData = request.Expenses.GroupBy(e => e.CategoryName).Select(g => (double)g.Sum(e => e.Cost)).ToArray();

            var chartData = request.Expenses.GroupBy(e => e.CategoryName).Select(g => new CategoryData { CategoryName = g.Key, Total = g.Sum(e => e.FortnightlyCost) }).ToList();
            request.BillOverviewData = chartData;
            request.BillOverviewChartOptions = new ApexChartOptions<CategoryData>
            {
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

            return request;
        }
    }

    public class CategoryData
    {
        public string CategoryName { get; set; } = "";
        public decimal Total { get; set; }
    }
}