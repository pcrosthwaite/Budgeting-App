using BudgetingApp.Data;
using BudgetingApp.Data.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetingApp.Web.Features.BankAccounts
{
    public class IndexQuery : IRequest<IndexQuery>
    {
        public List<IndexModel> Data { get; set; } = new();
    }

    /// <summary>
    /// <see cref="BankAccount"/>
    /// </summary>
    public class IndexModel
    {
        public int BankAccountId { get; set; }
        public string Name { get; set; }
        public string AccountNumber { get; set; }

        public string BankName => Bank?.Name ?? string.Empty;
        public int BankId { get; set; }
        public Bank Bank { get; set; }
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
            var data = await _context.BankAccounts
                                        .Include(x => x.Bank)
                                        .ToListAsync(cancellationToken);

            var result = _mapper.MapList<BankAccount, IndexModel>(data);

            request.Data = result.ToList();

            return request;
        }
    }
}