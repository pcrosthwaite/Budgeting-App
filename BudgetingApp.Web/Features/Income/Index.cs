using BudgetingApp.Data;
using BudgetingApp.Data.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetingApp.Web.Features.Income
{
    public class IndexQuery : IRequest<IndexQuery>
    {
        public List<IndexModel> Results { get; set; }
    }

    /// <summary>
    /// <see cref="Data.Models.Income"/>
    /// </summary>
    public class IndexModel
    {
        public int IncomeId { get; set; }

        public string PersonName { get; set; }
        public decimal Amount { get; set; }
        public TransactionFrequency Frequency { get; set; }
        public string CategoryName { get; set; }
    }

    public class IndexQueryHandler : IRequestHandler<IndexQuery, IndexQuery>
    {
        private readonly BudgetingDbContext _context;
        private readonly IMapperService _mapper;

        public IndexQueryHandler(BudgetingDbContext context, IMapperService mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IndexQuery> Handle(IndexQuery request, CancellationToken cancellationToken)
        {
            var data = await _context.Income
                                .Include(x => x.Person)
                                .ToListAsync(cancellationToken);

            request.Results = _mapper.MapList<Data.Models.Income, IndexModel>(data).ToList();

            return request;
        }
    }
}