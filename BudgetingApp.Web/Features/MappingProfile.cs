using BudgetingApp.Data.Models;
using Riok.Mapperly.Abstractions;

namespace BudgetingApp.Web.Features
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
}