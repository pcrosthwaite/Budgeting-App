using BudgetingApp.Data.Services;
using MediatR;

namespace BudgetingApp.Web.Features.Persons
{
    public class DeleteCommand : IRequest<bool>
    {
        public int PersonId { get; set; }
    }

    public class DeleteCommandHandler : IRequestHandler<DeleteCommand, bool>
    {
        private readonly PersonService _personService;

        public DeleteCommandHandler(PersonService personService)
        {
            _personService = personService;
        }

        public async Task<bool> Handle(DeleteCommand request, CancellationToken cancellationToken)
        {
            var result = await _personService.DeletePersonAsync(request.PersonId, cancellationToken);

            return result;
        }
    }
}