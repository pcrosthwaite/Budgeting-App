using BudgetingApp.Data;
using BudgetingApp.Data.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetingApp.Web.Features.Category
{
    public class IndexQuery : IRequest<IndexQuery>
    {
        public List<IndexModel> Data { get; set; } = new();
    }

    /// <summary>
    /// <see cref="Data.Models.Category"/>
    /// </summary>
    public class IndexModel
    {
        public int CategoryId { get; set; }
        public CategoryType CategoryType { get; set; }
        public string Name { get; set; }
        public string HexColourCode { get; set; }
        public string HexColourCodeStyle => HexColourCode.HasValue() ? $"background-color: {HexColourCode}; border-color: {HexColourCode};" : "";
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
            var categories = await _context.Categories.ToListAsync(cancellationToken);

            var result = _mapper.MapList<Data.Models.Category, IndexModel>(categories);

            request.Data = result.ToList();

            return request;
        }
    }
}