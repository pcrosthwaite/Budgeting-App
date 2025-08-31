using BudgetingApp.Data;
using BudgetingApp.Data.Models;
using BudgetingApp.Data.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetingApp.Web.Features.Income
{
    public class SaveQuery : IRequest<SaveCommand>
    {
        public int? IncomeId { get; set; }
    }

    public class SaveQueryHandler : IRequestHandler<SaveQuery, SaveCommand>
    {
        private readonly BudgetingDbContext _context;
        private readonly IMapperService _mapper;
        private readonly PersonService _personService;

        public SaveQueryHandler(BudgetingDbContext context, IMapperService mapper, PersonService personService)
        {
            _context = context;
            _mapper = mapper;
            _personService = personService;
        }

        public async Task<SaveCommand> Handle(SaveQuery request, CancellationToken cancellationToken)
        {
            var command = new SaveCommand();

            if (request.IncomeId != default)
            {
                var data = await _context.Income.FindAsync(request.IncomeId);

                if (data == null)
                {
                    throw new KeyNotFoundException($"Key not found {request.IncomeId}.");
                }

                _mapper.Map<Data.Models.Income, SaveCommand>(data, command);

                //command.PersonExpenses = await _context.PersonExpenses.Where(x => x.ExpenseId == request.ExpenseId).Select(x => new PersonExpenseModel
                //{
                //    PersonExpenseId = x.PersonExpenseId,
                //    PersonId = x.PersonId,
                //    PersonName = x.Person.Name,
                //    Percentage = x.Percentage
                //}).ToListAsync(cancellationToken);
            }

            command.Persons = await _personService.GetAllAsync(cancellationToken);
            command.IncomeCategories = await _context.Categories.Where(x => x.CategoryType == CategoryType.Income).ToListAsync(cancellationToken);

            if (request.IncomeId == default)
            {
                var category = command.IncomeCategories.Where(x => x.CategoryType == CategoryType.Income).FirstOrDefault();
                command.CategoryId = category?.CategoryId;
            }

            return command;
        }
    }

    /// <summary>
    /// <see cref="Data.Models.Income"/>
    /// </summary>
    public class SaveCommand : IRequest<int>
    {
        public int IncomeId { get; set; } // Primary Key
        public int? PersonId { get; set; }
        public string PersonName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public TransactionFrequency Frequency { get; set; } = TransactionFrequency.Fortnightly;
        public int? CategoryId { get; set; } // Add category ID to the SaveCommand
        //public Data.Models.Category ExpenseCategory { get; set; }

        // Navigation property for the many-to-many relationship
        //public List<PersonExpenseModel> PersonExpenses { get; set; } = new();

        public List<Person> Persons { get; set; }
        public List<Data.Models.Category> IncomeCategories { get; set; }
    }

    //public class PersonExpenseModel
    //{
    //    public int PersonExpenseId { get; set; }
    //    public int PersonId { get; set; }
    //    public string PersonName { get; set; }

    //    public int ExpenseId { get; set; }

    //    public double Percentage { get; set; }
    //}

    public class SaveCommandHandler : IRequestHandler<SaveCommand, int>
    {
        private readonly BudgetingDbContext _context;
        private readonly PersonService _personService;
        private readonly IMapperService _mapper;

        public SaveCommandHandler(BudgetingDbContext context, IMapperService mapper, PersonService personService)
        {
            _context = context;
            _personService = personService;
            _mapper = mapper;
        }

        public async Task<int> Handle(SaveCommand request, CancellationToken cancellationToken)
        {
            var dest = default(Data.Models.Income);
            //var existingPersonIds = new List<int>();

            if (request.IncomeId != default)
            {
                dest = await _context.Income.FindAsync(request.IncomeId);
                if (dest == null)
                {
                    throw new KeyNotFoundException($"Key not found {request.IncomeId}.");
                }

                //existingPersonIds = await _context.PersonExpenses
                //                                .Where(pe => pe.ExpenseId == request.ExpenseId)
                //                                .Select(pe => pe.PersonId)
                //                                .ToListAsync(cancellationToken);
            }
            else
            {
                dest = new Data.Models.Income();
            }

            _mapper.Map(request, dest);

            _context.AddOrUpdate(dest, x => x.IncomeId);

            await _context.SaveChangesAsync(cancellationToken);

            return dest.IncomeId;
        }
    }
}