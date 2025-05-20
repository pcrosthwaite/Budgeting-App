// needs to be root namespace
namespace BudgetingApp.Web
{
    public interface IMapperService
    {
        TDestination Map<TSource, TDestination>(TSource source) where TSource : class where TDestination : class;

        ICollection<TDestination> MapList<TSource, TDestination>(ICollection<TSource> sourceList) where TSource : class where TDestination : class;

        /// <summary>
        /// Merges the source object into the target object e.g. SaveCommand to Client
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        void Map<TSource, TDestination>(TSource source, TDestination target) where TSource : class where TDestination : class;
    }
}