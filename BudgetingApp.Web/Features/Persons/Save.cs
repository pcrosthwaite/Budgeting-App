using BudgetingApp.Data.Models;
using BudgetingApp.Data.Services;
using MediatR;

namespace BudgetingApp.Web.Features.Persons
{
    public class SaveQuery : IRequest<SaveCommand>
    {
        public int PersonId { get; set; }
    }

    public class SaveQueryHandler : IRequestHandler<SaveQuery, SaveCommand>
    {
        private readonly PersonService _personService;
        private readonly IMapperService _mapper;

        public SaveQueryHandler(PersonService personService, IMapperService mapper)
        {
            _personService = personService;
            _mapper = mapper;
        }

        public async Task<SaveCommand> Handle(SaveQuery request, CancellationToken cancellationToken)
        {
            var command = default(SaveCommand);

            if (request.PersonId != default)
            {
                var data = await _personService.GetPersonAsync(request.PersonId, cancellationToken);

                if (data == null)
                {
                    throw new KeyNotFoundException($"Key not found {request.PersonId}.");
                }

                command = _mapper.Map<Person, SaveCommand>(data);
            }
            else
            {
                command = new SaveCommand();
            }

            return command;
        }
    }

    public class SaveCommand : IRequest<int>
    {
        public int PersonId { get; set; }
        public string Name { get; set; }
    }

    public class SavePersonCommandHandler : IRequestHandler<SaveCommand, int>
    {
        private readonly PersonService _personService;
        private readonly IMapperService _mapper;

        public SavePersonCommandHandler(PersonService personService, IMapperService mapper)
        {
            _personService = personService;
            _mapper = mapper;
        }

        public async Task<int> Handle(SaveCommand request, CancellationToken cancellationToken)
        {
            var dest = default(Person);

            if (request.PersonId != default)
            {
                dest = await _personService.GetPersonAsync(request.PersonId, cancellationToken);

                if (dest == null)
                {
                    throw new KeyNotFoundException($"Key not found {request.PersonId}.");
                }
            }
            else
            {
                dest = new Person();
            }

            _mapper.Map<SaveCommand, Person>(request, dest);

            await _personService.SaveAsync(dest);

            return dest.PersonId;
        }
    }
}