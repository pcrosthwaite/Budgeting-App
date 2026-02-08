using BudgetingApp.Data;
using BudgetingApp.Data.Models;
using BudgetingApp.Data.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetingApp.Web.Features.Expenses
{
    public class SaveQuery : IRequest<SaveCommand>
    {
        public int? ExpenseId { get; set; }
    }

    public class SaveQueryHandler : IRequestHandler<SaveQuery, SaveCommand>
    {
        private readonly BudgetingDbContext _context;
        private readonly ExpenseService _expenseService;
        private readonly IMapperService _mapper;
        private readonly PersonService _personService;

        public SaveQueryHandler(BudgetingDbContext context, ExpenseService expenseService, IMapperService mapper, PersonService personService)
        {
            _context = context;
            _expenseService = expenseService;
            _mapper = mapper;
            _personService = personService;
        }

        public async Task<SaveCommand> Handle(SaveQuery request, CancellationToken cancellationToken)
        {
            var command = new SaveCommand();

            if (request.ExpenseId != default)
            {
                var data = await _expenseService.GetExpenseAsync(request.ExpenseId);

                if (data == null)
                {
                    throw new KeyNotFoundException($"Key not found {request.ExpenseId}.");
                }

                _mapper.Map<Expense, SaveCommand>(data, command);

                command.PersonExpenses = await _context.PersonExpenses.Where(x => x.ExpenseId == request.ExpenseId).Select(x => new PersonExpenseModel
                {
                    PersonExpenseId = x.PersonExpenseId,
                    PersonId = x.PersonId,
                    PersonName = x.Person.Name,
                    Percentage = x.Percentage
                }).ToListAsync(cancellationToken);
            }

            command.Persons = await _personService.GetAllAsync(cancellationToken);
            command.ExpenseCategories = await _expenseService.GetExpenseCategories(cancellationToken);
            command.BankAccounts = await _context.BankAccounts.ToListAsync(cancellationToken);

            return command;
        }
    }

    /// <summary>
    /// <see cref="Expense"/>
    /// </summary>
    public class SaveCommand : IRequest<int>
    {
        public int ExpenseId { get; set; } // Primary Key
        public string Name { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public TransactionFrequency Frequency { get; set; }
        public int? BankAccountId { get; set; }
        public bool IsSubscription { get; set; }

        public int? CategoryId { get; set; } // Add category ID to the SaveCommand
        public Data.Models.Category ExpenseCategory { get; set; }

        // Navigation property for the many-to-many relationship
        public List<PersonExpenseModel> PersonExpenses { get; set; } = new();

        public List<Person> Persons { get; set; }
        public List<Data.Models.Category> ExpenseCategories { get; set; } = new();
        public List<BankAccount> BankAccounts { get; set; } = new();
        public decimal FortnightlyCost { get; set; }
    }

    public class PersonExpenseModel
    {
        public int PersonExpenseId { get; set; }
        public int PersonId { get; set; }
        public string PersonName { get; set; }

        public int ExpenseId { get; set; }

        public double Percentage { get; set; }
    }

    public class SaveCommandHandler : IRequestHandler<SaveCommand, int>
    {
        private readonly BudgetingDbContext _context;
        private readonly BudgetService _budgetService;
        private readonly ExpenseService _expenseService;
        private readonly PersonService _personService;
        private readonly IMapperService _mapper;

        public SaveCommandHandler(BudgetingDbContext context, BudgetService budgetService, ExpenseService expenseService, IMapperService mapper, PersonService personService)
        {
            _context = context;
            _budgetService = budgetService;
            _expenseService = expenseService;
            _personService = personService;
            _mapper = mapper;
        }

        public async Task<int> Handle(SaveCommand request, CancellationToken cancellationToken)
        {
            var dest = default(Expense);
            var existingPersonIds = new List<int>();
            var shouldTrackCostChange = false;
            var previousCost = 0m;

            if (request.ExpenseId != default)
            {
                dest = await _expenseService.GetExpenseAsync(request.ExpenseId);
                if (dest == null)
                {
                    throw new KeyNotFoundException($"Key not found {request.ExpenseId}.");
                }

                if (dest.Cost != request.Cost)
                {
                    shouldTrackCostChange = true;
                    previousCost = dest.Cost;
                }

                existingPersonIds = await _context.PersonExpenses
                                                .Where(pe => pe.ExpenseId == request.ExpenseId)
                                                .Select(pe => pe.PersonId)
                                                .ToListAsync(cancellationToken);
            }
            else
            {
                dest = new Expense();
                shouldTrackCostChange = true;
            }

            _mapper.Map(request, dest);

            _context.AddOrUpdate(dest, x => x.ExpenseId);

            await _context.SaveChangesAsync();

            if (shouldTrackCostChange)
            {
                var expenseHistory = new Data.Models.ExpenseHistory
                {
                    ExpenseId = dest.ExpenseId,
                    Cost = dest.Cost,
                    ChangedDate = DateTime.UtcNow,
                    Notes = request.ExpenseId == default
                        ? "Initial expense creation"
                        : $"Cost changed from ${previousCost:F2} to ${dest.Cost:F2}"
                };

                _context.AddOrUpdate(expenseHistory, x => x.ExpenseHistoryId);

                await _context.SaveChangesAsync();
            }

            // Handle new persons in the request
            var newPersons = request.PersonExpenses
                .Where(pe => pe.PersonId == default)
                .Select(pe => new Person
                {
                    Name = pe.PersonName,
                })
                .ToList();

            if (newPersons.Any())
            {
                var addedPersons = await _budgetService.AddPersonsAsync(newPersons, cancellationToken);

                foreach (var addedPerson in addedPersons)
                {
                    var personExpense = request.PersonExpenses.First(pe => pe.PersonName.Equals(addedPerson.Name, StringComparison.OrdinalIgnoreCase));
                    personExpense.PersonId = addedPerson.PersonId;
                }
            }

            // Identify and remove unnecessary PersonExpense records
            var newPersonIds = request.PersonExpenses.Select(pe => pe.PersonId).ToList();
            var personIdsToRemove = existingPersonIds.Except(newPersonIds).ToList();

            if (personIdsToRemove.Any())
            {
                await _expenseService.RemoveExpenseResponsiblePersonsAsync(dest.ExpenseId, personIdsToRemove);
            }

            // Add/Update the person expenses. for some reason, this can't be done from the expenseService
            foreach (var personExpense in request.PersonExpenses)
            {
                var destPersonExpense = default(PersonExpense);

                if (personExpense.PersonExpenseId != default)
                {
                    destPersonExpense = await _context.PersonExpenses.FirstOrDefaultAsync(x => x.PersonExpenseId == personExpense.PersonExpenseId, cancellationToken);
                }
                else
                {
                    if (personExpense.PersonId == default)
                    {
                        var persons = await _personService.GetAllAsync(cancellationToken);

                        var person = persons?.FirstOrDefault(x => x.Name.Equals(personExpense.PersonName, StringComparison.OrdinalIgnoreCase));

                        if (person != null)
                        {
                            personExpense.PersonId = person.PersonId;
                        }
                        else
                        {
                            throw new KeyNotFoundException($"Person with name {personExpense.PersonName} not found.");
                        }
                    }

                    destPersonExpense = await _context.PersonExpenses.FirstOrDefaultAsync(x => x.ExpenseId == dest.ExpenseId && x.PersonId == personExpense.PersonId, cancellationToken);

                    destPersonExpense ??= new PersonExpense();
                }

                destPersonExpense.ExpenseId = dest.ExpenseId;
                destPersonExpense.PersonId = personExpense.PersonId;
                destPersonExpense.Percentage = personExpense.Percentage;

                _context.AddOrUpdate(destPersonExpense, x => x.PersonExpenseId);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return dest.ExpenseId;
        }
    }
}