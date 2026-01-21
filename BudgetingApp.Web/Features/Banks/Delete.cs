using BudgetingApp.Data;
using BudgetingApp.Data.Services;
using MediatR;

namespace BudgetingApp.Web.Features.Banks
{
    public class DeleteCommand : IRequest<bool>
    {
        public int BankId { get; set; }
    }

    public class DeleteCommandHandler : IRequestHandler<DeleteCommand, bool>
    {
        private readonly BudgetingDbContext _context;

        public DeleteCommandHandler(BudgetingDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteCommand request, CancellationToken cancellationToken)
        {
            var data = await _context.Banks.FindAsync(request.BankId, cancellationToken);

            _context.Remove(data);

            var resultCount = await _context.SaveChangesAsync(cancellationToken);

            return resultCount == 1;
        }
    }
}