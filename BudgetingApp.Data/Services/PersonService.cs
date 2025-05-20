using BudgetingApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BudgetingApp.Data.Services
{
    public class PersonService
    {
        private readonly BudgetingDbContext _context;

        public PersonService(BudgetingDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task AddOrUpdatePersonAsync(Person person)
        {
            var existingPerson = await _context.Persons.FirstOrDefaultAsync(p => p.Name.ToLower() == person.Name.ToLower());
            if (existingPerson != null && person.PersonId != existingPerson.PersonId)
            {
                throw new InvalidOperationException($"A person with the name '{person.Name}' already exists.");
            }

            _context.AddOrUpdate(person, p => p.PersonId);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeletePersonAsync(int personId, CancellationToken cancellationToken = default)
        {
            var data = await _context.Persons.FindAsync(personId, cancellationToken);

            _context.Remove(data);

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<List<Person>> GetAllAsync(CancellationToken cancellationToken)
        {
            var results = await _context.Persons.ToListAsync(cancellationToken);

            return results;
        }

        public async Task<Person> GetPersonAsync(int personId, CancellationToken cancellationToken)
        {
            var result = await _context.Persons.FirstOrDefaultAsync(p => p.PersonId == personId, cancellationToken);

            return result;
        }

        public async Task SaveAsync(Person dest)
        {
            _context.AddOrUpdate(dest, x => x.PersonId);

            await _context.SaveChangesAsync();
        }
    }
}