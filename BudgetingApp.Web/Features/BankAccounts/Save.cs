using BudgetingApp.Data;
using BudgetingApp.Data.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetingApp.Web.Features.BankAccounts
{
    public class SaveQuery : IRequest<SaveCommand>
    {
        public int? BankAccountId { get; set; }
    }

    public class SaveQueryHandler : IRequestHandler<SaveQuery, SaveCommand>
    {
        private readonly BudgetingDbContext _context;
        private readonly IMapperService _mapper;

        public SaveQueryHandler(BudgetingDbContext context, IMapperService mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<SaveCommand> Handle(SaveQuery request, CancellationToken cancellationToken)
        {
            var command = default(SaveCommand);

            if (request.BankAccountId.HasValue)
            {
                var data = await _context.BankAccounts.FindAsync(request.BankAccountId, cancellationToken);

                if (data == null)
                {
                    throw new KeyNotFoundException($"Key not found {request.BankAccountId}.");
                }

                command = _mapper.Map<BankAccount, SaveCommand>(data);
            }
            else
            {
                command = new SaveCommand();
            }

            command.BankList = await _context.Banks.ToListAsync(cancellationToken);

            return command;
        }
    }

    /// <summary>
    /// <see cref="BankAccount" />
    /// </summary>
    public class SaveCommand : IRequest<int>
    {
        public int BankAccountId { get; set; }
        public string Name { get; set; }
        public string AccountNumber { get; set; }
        public int BankId { get; set; }

        public List<Bank> BankList { get; set; } = new();
    }

    public class SavePersonCommandHandler : IRequestHandler<SaveCommand, int>
    {
        private readonly BudgetingDbContext _context;
        private readonly IMapperService _mapper;

        public SavePersonCommandHandler(BudgetingDbContext context, IMapperService mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<int> Handle(SaveCommand request, CancellationToken cancellationToken)
        {
            var dest = default(BankAccount);

            if (request.BankAccountId != default)
            {
                dest = await _context.BankAccounts.FindAsync(request.BankAccountId, cancellationToken);

                if (dest == null)
                {
                    throw new KeyNotFoundException($"Key not found {request.BankAccountId}.");
                }
            }
            else
            {
                dest = new BankAccount();
            }

            _mapper.Map<SaveCommand, BankAccount>(request, dest);

            _context.AddOrUpdate(dest, x => x.BankAccountId);

            await _context.SaveChangesAsync(cancellationToken);

            return dest.BankAccountId;
        }
    }
}