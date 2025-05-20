using BudgetingApp.Data.Models;
using BudgetingApp.Data.Services;
using MediatR;

namespace BudgetingApp.Web.Features.Persons
{
    public class IndexQuery : IRequest<IndexQuery>
    {
        public List<IndexModel> Persons { get; set; } = new();
    }

    public class IndexModel
    {
        public int PersonId { get; set; }
        public string Name { get; set; }
    }

    public class IndexQueryHandler : IRequestHandler<IndexQuery, IndexQuery>
    {
        private readonly PersonService _personService;
        private readonly IMapperService _mapper;

        public IndexQueryHandler(PersonService personService, IMapperService mapper)
        {
            _personService = personService;
            _mapper = mapper;
        }

        public async Task<IndexQuery> Handle(IndexQuery request, CancellationToken cancellationToken)
        {
            var persons = await _personService.GetAllAsync(cancellationToken);

            request.Persons = _mapper.MapList<Person, IndexModel>(persons).ToList();

            return request;
        }
    }
}