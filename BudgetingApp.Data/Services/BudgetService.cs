using BudgetingApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BudgetingApp.Data.Services
{
    public class BudgetService
    {
        private readonly BudgetingDbContext _context;

        public BudgetService(BudgetingDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<Person> AddPersonAsync(Person person, CancellationToken cancellationToken)
        {
            if (person == null) return person;

            _context.Persons.Add(person);

            await _context.SaveChangesAsync(cancellationToken);

            return person;
        }

        public async Task<List<Person>> AddPersonsAsync(List<Person> personsToAdd, CancellationToken cancellationToken)
        {
            var filteredList = personsToAdd.Where(x => x.PersonId == default).ToList();

            foreach (var person in filteredList)
            {
                _context.Persons.Add(person);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return filteredList;
        }
    }

    //public class GetExpensesQuery : IRequest<IEnumerable<Expense>>
    //{ }

    //public class GetExpensesQueryHandler : IRequestHandler<GetExpensesQuery, IEnumerable<Expense>>
    //{
    //    private readonly BudgetService _budgetService;

    //    public GetExpensesQueryHandler(BudgetService budgetService)
    //    {
    //        _budgetService = budgetService;
    //    }

    //    public async Task<IEnumerable<Expense>> Handle(GetExpensesQuery request, CancellationToken cancellationToken)
    //    {
    //        return await _budgetService.GetExpensesAsync();
    //    }
    //}

    //public class AddExpenseCommand : IRequest
    //{
    //    public Expense Expense { get; set; } = null!;
    //}

    //public class AddExpenseCommandHandler : IRequestHandler<AddExpenseCommand>
    //{
    //    private readonly BudgetService _budgetService;

    //    public AddExpenseCommandHandler(BudgetService budgetService)
    //    {
    //        _budgetService = budgetService;
    //    }

    //    public async Task<Unit> Handle(AddExpenseCommand request, CancellationToken cancellationToken)
    //    {
    //        await _budgetService.AddUpdateExpenseAsync(request.Expense, e => e.ExpenseId);
    //        return Unit.Value;
    //    }
    //}
}