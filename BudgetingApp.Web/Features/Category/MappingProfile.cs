using Riok.Mapperly.Abstractions;

namespace BudgetingApp.Web.Features.Category
{
    [Mapper]
    public partial class CategoryMapper : IMapperClass<Data.Models.Category, IndexModel>
    {
        //[MapProperty(nameof(Client.PrimaryEmail), nameof(IndexModel.PrimaryContactEmail))]
        //[MapProperty(nameof(Client.PrimaryContact.FullName), nameof(IndexModel.PrimaryContactName))]
        public partial IndexModel MapTo(Data.Models.Category client);

        public partial ICollection<IndexModel> MapToList(ICollection<Data.Models.Category> clientList);

        public partial void Merge(Data.Models.Category source, IndexModel target);
    }

    [Mapper]
    public partial class CategorySaveCommandMapper : IMapperClass<Data.Models.Category, SaveCommand>
    {
        public partial SaveCommand MapTo(Data.Models.Category client);

        public partial ICollection<SaveCommand> MapToList(ICollection<Data.Models.Category> clientList);

        public partial void Merge(Data.Models.Category source, SaveCommand target);
    }

    [Mapper]
    public partial class SaveCommandCategoryMapper : IMapperClass<SaveCommand, Data.Models.Category>
    {
        public partial Data.Models.Category MapTo(SaveCommand client);

        public partial void Merge(SaveCommand source, Data.Models.Category target);

        public partial ICollection<Data.Models.Category> MapToList(ICollection<SaveCommand> sourceList);
    }
}