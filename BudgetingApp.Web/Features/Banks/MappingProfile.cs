using BudgetingApp.Data.Models;
using BudgetingApp.Web.Features.Expenses;
using Riok.Mapperly.Abstractions;

namespace BudgetingApp.Web.Features.Banks
{
    [Mapper]
    public partial class BankMapper : IMapperClass<Bank, IndexModel>
    {
        //[MapProperty(nameof(Client.PrimaryEmail), nameof(IndexModel.PrimaryContactEmail))]
        //[MapProperty(nameof(Client.PrimaryContact.FullName), nameof(IndexModel.PrimaryContactName))]
        public partial IndexModel MapTo(Bank data);

        public partial ICollection<IndexModel> MapToList(ICollection<Bank> dataList);

        public partial void Merge(Bank source, IndexModel target);
    }

    [Mapper]
    public partial class BankSaveCommandMapper : IMapperClass<Bank, SaveCommand>
    {
        public partial SaveCommand MapTo(Bank data);

        public partial ICollection<SaveCommand> MapToList(ICollection<Bank> dataList);

        public partial void Merge(Bank source, SaveCommand target);
    }

    [Mapper]
    public partial class SaveCommandBankMapper : IMapperClass<SaveCommand, Bank>
    {
        public partial Bank MapTo(SaveCommand client);

        public partial void Merge(SaveCommand source, Bank target);

        public partial ICollection<Bank> MapToList(ICollection<SaveCommand> sourceList);
    }
}