using BudgetingApp.Data;
using BudgetingApp.Data.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetingApp.Web.Features.Category
{
    public class SaveQuery : IRequest<SaveCommand>
    {
        public int? CategoryId { get; set; }
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

            if (request.CategoryId.HasValue)
            {
                var data = await _context.Categories.FirstOrDefaultAsync(x => x.CategoryId == request.CategoryId, cancellationToken);

                if (data == null)
                {
                    throw new KeyNotFoundException($"Key not found {request.CategoryId}.");
                }

                command = _mapper.Map<Data.Models.Category, SaveCommand>(data);
            }
            else
            {
                command = new SaveCommand();
            }

            return command;
        }
    }

    /// <summary>
    /// <see cref="Data.Models.Category" />
    /// </summary>
    public class SaveCommand : IRequest<int>
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string HexColourCode { get; set; }

        public CategoryType CategoryType { get; set; }
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
            var dest = default(Data.Models.Category);

            if (request.CategoryId != default)
            {
                dest = await _context.Categories.FirstOrDefaultAsync(x => x.CategoryId == request.CategoryId, cancellationToken);

                if (dest == null)
                {
                    throw new KeyNotFoundException($"Key not found {request.CategoryId}.");
                }
            }
            else
            {
                dest = new Data.Models.Category();
            }

            _mapper.Map<SaveCommand, Data.Models.Category>(request, dest);

            _context.AddOrUpdate(dest, x => x.CategoryId);

            await _context.SaveChangesAsync(cancellationToken);

            return dest.CategoryId;
        }
    }
}