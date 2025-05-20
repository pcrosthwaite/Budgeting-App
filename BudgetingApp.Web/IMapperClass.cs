// needs to be root namespace
namespace BudgetingApp.Web
{
    public interface IMapperClass<TSource, TDestination> where TSource : class where TDestination : class
    {
        TDestination MapTo(TSource source);

        ICollection<TDestination> MapToList(ICollection<TSource> sourceList);

        void Merge(TSource source, TDestination target);
    }
}