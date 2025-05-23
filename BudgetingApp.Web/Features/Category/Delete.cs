using BudgetingApp.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetingApp.Web.Features.Category
{
    public class DeleteCommand : IRequest<bool>
    {
        public int CategoryId { get; set; }
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
            var data = await _context.Categories.FirstOrDefaultAsync(x => x.CategoryId == request.CategoryId, cancellationToken);

            _context.Remove(data);

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}