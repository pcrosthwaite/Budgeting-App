using BudgetingApp.Data.Models;
using BudgetingApp.Data.Services;
using MediatR;

namespace BudgetingApp.Web.Features.Expenses
{
    public class IndexQuery : IRequest<IndexQuery>
    {
        public List<IndexModel> Expenses { get; set; } = new();
    }

    public class IndexModel
    {
        public int ExpenseId { get; set; } // Primary Key
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public decimal Cost { get; set; }
        public TransactionFrequency Frequency { get; set; }
        public bool IncludeInBillsAccount { get; set; }
        public bool IsSubscription { get; set; }

        // Navigation property for the many-to-many relationship
        public List<PersonExpense> PersonExpenses { get; set; }

        public decimal FortnightlyCost { get; set; }
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

            return request;
        }
    }
}