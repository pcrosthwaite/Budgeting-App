using BudgetingApp.Data;
using BudgetingApp.Data.Models;
using BudgetingApp.Data.Services;
using MediatR;

namespace BudgetingApp.Web.Features.Banks
{
    public class SaveQuery : IRequest<SaveCommand>
    {
        public int BankId { get; set; }
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

            if (request.BankId != default)
            {
                var data = await _context.Banks.FindAsync(request.BankId, cancellationToken);

                if (data == null)
                {
                    throw new KeyNotFoundException($"Key not found {request.BankId}.");
                }

                command = _mapper.Map<Bank, SaveCommand>(data);
            }
            else
            {
                command = new SaveCommand();
            }

            return command;
        }
    }

    /// <summary>
    /// <see cref="Bank"/>
    /// </summary>
    public class SaveCommand : IRequest<int>
    {
        public int BankId { get; set; }
        public string Name { get; set; }
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
            var dest = default(Bank);

            if (request.BankId != default)
            {
                dest = await _context.Banks.FindAsync(request.BankId, cancellationToken);

                if (dest == null)
                {
                    throw new KeyNotFoundException($"Key not found {request.BankId}.");
                }
            }
            else
            {
                dest = new Bank();
            }

            _mapper.Map<SaveCommand, Bank>(request, dest);

            _context.AddOrUpdate(dest, x => x.BankId);

            await _context.SaveChangesAsync(cancellationToken);

            return dest.BankId;
        }
    }
}