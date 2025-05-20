using BudgetingApp.Data.Services;
using MediatR;

namespace BudgetingApp.Web.Features.Expenses
{
    public class DeleteCommand : IRequest<bool>
    {
        public int ExpenseId { get; set; }
    }

    public class DeleteCommandHandler : IRequestHandler<DeleteCommand, bool>
    {
        private readonly BudgetService _budgetService;
        private readonly ExpenseService _expenseService;

        public DeleteCommandHandler(BudgetService budgetService, ExpenseService expenseService)
        {
            _budgetService = budgetService;
            _expenseService = expenseService;
        }

        public async Task<bool> Handle(DeleteCommand request, CancellationToken cancellationToken)
        {
            var result = await _expenseService.DeleteExpenseAsync(request.ExpenseId);

            return result;
        }
    }
}