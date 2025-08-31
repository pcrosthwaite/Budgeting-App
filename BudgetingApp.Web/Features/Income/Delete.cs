using BudgetingApp.Data;
using BudgetingApp.Data.Services;
using MediatR;

namespace BudgetingApp.Web.Features.Income
{
    public class DeleteCommand : IRequest<bool>
    {
        public int IncomeId { get; set; }
    }

    public class DeleteCommandHandler : IRequestHandler<DeleteCommand, bool>
    {
        public readonly BudgetingDbContext _context;

        public DeleteCommandHandler(BudgetingDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteCommand request, CancellationToken cancellationToken)
        {
            var data = await _context.Income.FindAsync(request.IncomeId);

            _context.Income.Remove(data);

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}