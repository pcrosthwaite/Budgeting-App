using BudgetingApp.Data;
using BudgetingApp.Data.Models;
using BudgetingApp.Data.Services;
using MediatR;

namespace BudgetingApp.Web.Features.Expenses
{
    public class SaveQuery : IRequest<SaveCommand>
    {
        public int? ExpenseId { get; set; }
    }

    public class SaveQueryHandler : IRequestHandler<SaveQuery, SaveCommand>
    {
        private readonly ExpenseService _expenseService;
        private readonly IMapperService _mapper;
        private readonly PersonService _personService;

        public SaveQueryHandler(ExpenseService expenseService, IMapperService mapper, PersonService personService)
        {
            _expenseService = expenseService;
            _mapper = mapper;
            _personService = personService;
        }

        public async Task<SaveCommand> Handle(SaveQuery request, CancellationToken cancellationToken)
        {
            //if (request.ExpenseId.HasValue)
            //{
            //    expense = await _expenseService.GetExpenseAsync(request.ExpenseId.Value);
            //    if (expense == null)
            //    {
            //        throw new Exception($"Expense with ID {request.ExpenseId.Value} not found.");
            //    }
            //}

            //var result = expense != null ? _mapper.ToSaveCommand(expense) : new SaveCommand();

            //result.Persons = await _personService.GetAllAsync(cancellationToken);

            //return result;

            var command = default(SaveCommand);

            if (request.ExpenseId != default)
            {
                var data = await _expenseService.GetExpenseAsync(request.ExpenseId);

                if (data == null)
                {
                    throw new KeyNotFoundException($"Key not found {request.ExpenseId}.");
                }

                command = _mapper.Map<Expense, SaveCommand>(data);
            }
            else
            {
                command = new SaveCommand();
            }

            return command;
        }
    }

    public class SaveCommand : IRequest<int>
    {
        public int ExpenseId { get; set; } // Primary Key
        public string Name { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public ExpenseFrequency Frequency { get; set; }
        public bool IncludeInBillsAccount { get; set; }

        // Navigation property for the many-to-many relationship
        public List<PersonExpense> PersonExpenses { get; set; } = new();

        public List<Person> Persons { get; set; }
        public decimal FortnightlyCost { get; set; }
    }

    public class SaveCommandHandler : IRequestHandler<SaveCommand, int>
    {
        private readonly BudgetingDbContext _context;
        private readonly BudgetService _budgetService;
        private readonly ExpenseService _expenseService;
        private readonly IMapperService _mapper;

        public SaveCommandHandler(BudgetingDbContext context, BudgetService budgetService, ExpenseService expenseService, IMapperService mapper)
        {
            _context = context;
            _budgetService = budgetService;
            _expenseService = expenseService;
            _mapper = mapper;
        }

        public async Task<int> Handle(SaveCommand request, CancellationToken cancellationToken)
        {
            // Step 1: Fetch or create the expense
            var expense = request.ExpenseId != 0
                ? await _expenseService.GetExpenseAsync(request.ExpenseId) ?? throw new Exception($"Expense with ID {request.ExpenseId} not found.")
                : new Expense();

            // Step 2: Retrieve existing PersonExpense records before mapping
            var existingPersonIds = expense.PersonExpenses.Select(pe => pe.PersonId).ToList();

            // Step 3: Map request data to the expense
            _mapper.Map(request, expense);

            _context.AddOrUpdate(expense, x => x.ExpenseId);

            await _context.SaveChangesAsync();

            // Step 4: Save or update the expense in the database
            //var savedExpense = await _budgetService.AddUpdateExpenseAsync(expense, e => e.ExpenseId);

            // Step 5: Handle new persons in the request
            var newPersons = request.PersonExpenses
                .Where(pe => pe.Person.PersonId == default)
                .Select(pe => pe.Person)
                .ToList();

            if (newPersons.Any())
            {
                var addedPersons = await _budgetService.AddPersonsAsync(newPersons, cancellationToken);

                foreach (var addedPerson in addedPersons)
                {
                    var personExpense = request.PersonExpenses.First(pe => pe.Person.Name.Equals(addedPerson.Name, StringComparison.OrdinalIgnoreCase));
                    personExpense.PersonId = addedPerson.PersonId;
                    personExpense.Person = addedPerson;
                }
            }

            // Step 6: Identify and remove unnecessary PersonExpense records
            var newPersonIds = request.PersonExpenses.Select(pe => pe.Person.PersonId).ToList();
            var personIdsToRemove = existingPersonIds.Except(newPersonIds).ToList();

            if (personIdsToRemove.Any())
            {
                await _expenseService.RemoveExpenseResponsiblePersonsAsync(expense.ExpenseId, personIdsToRemove);
            }

            // Step 7: Add or Update PersonExpense records
            await _expenseService.AddUpdatePersonExpensesAsync(expense.ExpenseId, request.PersonExpenses);

            return expense.ExpenseId;
        }
    }
}