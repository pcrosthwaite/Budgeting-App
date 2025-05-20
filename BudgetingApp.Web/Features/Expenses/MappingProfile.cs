using BudgetingApp.Data.Models;
using Riok.Mapperly.Abstractions;

namespace BudgetingApp.Web.Features.Expenses
{
    [Mapper]
    public partial class ExpenseMapper : IMapperClass<Expense, IndexModel>
    {
        //[MapProperty(nameof(Client.PrimaryEmail), nameof(IndexModel.PrimaryContactEmail))]
        //[MapProperty(nameof(Client.PrimaryContact.FullName), nameof(IndexModel.PrimaryContactName))]
        public partial IndexModel MapTo(Expense client);

        public partial ICollection<IndexModel> MapToList(ICollection<Expense> clientList);

        public partial void Merge(Expense source, IndexModel target);
    }

    [Mapper]
    public partial class ExpenseSaveCommandMapper : IMapperClass<Expense, SaveCommand>
    {
        public partial SaveCommand MapTo(Expense client);

        public partial ICollection<SaveCommand> MapToList(ICollection<Expense> clientList);

        public partial void Merge(Expense source, SaveCommand target);
    }

    [Mapper]
    public partial class SaveCommandExpenseMapper : IMapperClass<SaveCommand, Expense>
    {
        public partial Expense MapTo(SaveCommand client);

        public partial void Merge(SaveCommand source, Expense target);

        public partial ICollection<Expense> MapToList(ICollection<SaveCommand> sourceList);
    }
}