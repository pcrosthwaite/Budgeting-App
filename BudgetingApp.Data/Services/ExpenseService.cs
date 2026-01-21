using BudgetingApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BudgetingApp.Data.Services
{
    public class ExpenseService
    {
        private readonly BudgetingDbContext _context;
        private readonly PersonService _personService;

        public ExpenseService(BudgetingDbContext context, PersonService personService)
        {
            _context = context;
            _personService = personService;
        }

        public async Task<ICollection<Expense>> GetExpensesAsync()
        {
            return await _context.Expenses
                            .Include(x => x.Category)
                            .Include(x => x.BankAccount)
                            .Include(e => e.PersonExpenses)
                                .ThenInclude(pe => pe.Person)
                            .ToListAsync();
        }

        public async Task<Expense> GetExpenseAsync(int? expenseId)
        {
            if (!expenseId.HasValue) return null;

            return await _context.Expenses
                            .Include(e => e.Category)
                            //.Include(e => e.PersonExpenses)
                            //    .ThenInclude(pe => pe.Person)
                            .FirstOrDefaultAsync(e => e.ExpenseId == expenseId.Value);
        }

        public async Task<Expense> AddUpdateExpenseAsync(Expense expense, Func<Expense, int> idSelector)
        {
            _context.AddOrUpdate(expense, idSelector);

            await _context.SaveChangesAsync();

            return expense; // Return the updated expense with the updated ExpenseId
        }

        /// <summary>
        /// Removes expense and any associated person expenses from the database.
        /// </summary>
        /// <param name="expense"></param>
        /// <returns></returns>
        public async Task<bool> DeleteExpenseAsync(Expense expense)
        {
            if (expense == null) return true;

            _context.Expenses.Remove(expense);

            await _context.SaveChangesAsync();

            await DeletePersonExpenseByExpenseIdAsync(expense.ExpenseId);

            return true;
        }

        /// <summary>
        /// Removes expense and any associated person expenses from the database.
        /// </summary>
        /// <param name="expenseId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteExpenseAsync(int expenseId)
        {
            var expense = await _context.Expenses.FindAsync(expenseId);

            // this caters for the case when the expense is not found
            var result = await DeleteExpenseAsync(expense);

            return result;
        }

        public async Task<bool> DeletePersonExpenseByExpenseIdAsync(int expenseId)
        {
            var personExpenses = await _context.PersonExpenses.Where(x => x.ExpenseId == expenseId).Select(x => x.PersonId).ToListAsync();

            // this caters for the case when the expense is not found
            var result = await RemoveExpenseResponsiblePersonsAsync(expenseId, personExpenses);

            return result;
        }

        public async Task<bool> RemoveExpenseResponsiblePersonsAsync(int expenseId, List<int> personIdsToRemove)
        {
            var existingRecords = await _context.PersonExpenses
                                            .Where(pe => pe.ExpenseId == expenseId && personIdsToRemove.Contains(pe.PersonId))
                                            .ToListAsync();

            foreach (var record in existingRecords)
            {
                _context.Remove(record);
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<Category>> GetExpenseCategories(CancellationToken cancellationToken)
        {
            var result = await GetCategories(CategoryType.Expense, cancellationToken);

            return result;
        }

        public async Task<List<Category>> GetIncomeCategories(CancellationToken cancellationToken)
        {
            var result = await GetCategories(CategoryType.Income, cancellationToken);

            return result;
        }

        private async Task<List<Category>> GetCategories(CategoryType type, CancellationToken cancellationToken)
        {
            var result = await _context.Categories.Where(x => x.CategoryType == type).ToListAsync();

            return result;
        }
    }
}