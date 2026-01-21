using BudgetingApp.Data;
using BudgetingApp.Data.Models;
using BudgetingApp.Data.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetingApp.Web.Features.Banks
{
    public class IndexQuery : IRequest<IndexQuery>
    {
        public List<IndexModel> Results { get; set; } = new();
    }

    /// <summary>
    /// <see cref="Bank"/>
    /// </summary>
    public class IndexModel
    {
        public int BankId { get; set; }
        public string Name { get; set; }
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
            var banks = await _context.Banks.ToListAsync(cancellationToken);

            request.Results = _mapper.MapList<Bank, IndexModel>(banks).ToList();

            return request;
        }
    }
}