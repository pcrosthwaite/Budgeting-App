using AutoMapper;
using Riok.Mapperly.Abstractions;

namespace BudgetingApp.Web.Features.Income
{
    [Mapper]
    public partial class IncomeMapper : IMapperClass<Data.Models.Income, IndexModel>
    {
        //[MapProperty(nameof(Data.Models.Income.Person.Name), nameof(IndexModel.PersonName))]
        public partial IndexModel MapTo(Data.Models.Income data);

        public partial ICollection<IndexModel> MapToList(ICollection<Data.Models.Income> dataList);

        public partial void Merge(Data.Models.Income source, IndexModel target);
    }

    [Mapper]
    public partial class IncomeSaveCommandMapper : IMapperClass<Data.Models.Income, SaveCommand>
    {
        public partial SaveCommand MapTo(Data.Models.Income client);

        public partial ICollection<SaveCommand> MapToList(ICollection<Data.Models.Income> clientList);

        public partial void Merge(Data.Models.Income source, SaveCommand target);
    }

    [Mapper]
    public partial class SaveCommandIncomeMapper : IMapperClass<SaveCommand, Data.Models.Income>
    {
        public partial Data.Models.Income MapTo(SaveCommand client);

        public partial void Merge(SaveCommand source, Data.Models.Income target);

        public partial ICollection<Data.Models.Income> MapToList(ICollection<SaveCommand> sourceList);
    }
}