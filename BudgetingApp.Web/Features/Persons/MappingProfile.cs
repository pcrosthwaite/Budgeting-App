using BudgetingApp.Data.Models;
using BudgetingApp.Web.Features.Expenses;
using Riok.Mapperly.Abstractions;

namespace BudgetingApp.Web.Features.Persons
{
    [Mapper]
    public partial class PersonMapper : IMapperClass<Person, IndexModel>
    {
        //[MapProperty(nameof(Client.PrimaryEmail), nameof(IndexModel.PrimaryContactEmail))]
        //[MapProperty(nameof(Client.PrimaryContact.FullName), nameof(IndexModel.PrimaryContactName))]
        public partial IndexModel MapTo(Person client);

        public partial ICollection<IndexModel> MapToList(ICollection<Person> clientList);

        public partial void Merge(Person source, IndexModel target);
    }

    [Mapper]
    public partial class PersonSaveCommandMapper : IMapperClass<Person, SaveCommand>
    {
        public partial SaveCommand MapTo(Person client);

        public partial ICollection<SaveCommand> MapToList(ICollection<Person> clientList);

        public partial void Merge(Person source, SaveCommand target);
    }

    [Mapper]
    public partial class SaveCommandPersonMapper : IMapperClass<SaveCommand, Person>
    {
        public partial Person MapTo(SaveCommand client);

        public partial void Merge(SaveCommand source, Person target);

        public partial ICollection<Person> MapToList(ICollection<SaveCommand> sourceList);
    }
}