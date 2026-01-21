using BudgetingApp.Data.Models;
using Riok.Mapperly.Abstractions;

namespace BudgetingApp.Web.Features.BankAccounts
{
    [Mapper]
    public partial class BankAccountMapper : IMapperClass<BankAccount, IndexModel>
    {
        //[MapProperty(nameof(Client.PrimaryEmail), nameof(IndexModel.PrimaryContactEmail))]
        //[MapProperty(nameof(Client.PrimaryContact.FullName), nameof(IndexModel.PrimaryContactName))]
        public partial IndexModel MapTo(BankAccount data);

        public partial ICollection<IndexModel> MapToList(ICollection<BankAccount> dataList);

        public partial void Merge(BankAccount source, IndexModel target);
    }

    [Mapper]
    public partial class BankAccountSaveCommandMapper : IMapperClass<BankAccount, SaveCommand>
    {
        public partial SaveCommand MapTo(BankAccount data);

        public partial ICollection<SaveCommand> MapToList(ICollection<BankAccount> dataList);

        public partial void Merge(BankAccount source, SaveCommand target);
    }

    [Mapper]
    public partial class SaveCommandBankAccountMapper : IMapperClass<SaveCommand, BankAccount>
    {
        public partial BankAccount MapTo(SaveCommand client);

        public partial void Merge(SaveCommand source, BankAccount target);

        public partial ICollection<BankAccount> MapToList(ICollection<SaveCommand> sourceList);
    }
}