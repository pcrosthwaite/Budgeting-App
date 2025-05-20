using System.Linq.Expressions;

namespace BudgetingApp.Web.Services
{
	public class FluentMapperConfigurator<TSource, TTarget>
	{
		private readonly List<MappingConfiguration<TSource, TTarget>> _mappings = new();

		public FluentMapperConfigurator<TSource, TTarget> ForMember(
			Expression<Func<TTarget, object>> targetSelector,
			Expression<Func<TSource, object>> sourceSelector)
		{
			_mappings.Add(new MappingConfiguration<TSource, TTarget>
			{
				SourceSelector = sourceSelector,
				TargetSelector = targetSelector,
				CustomMapFunction = null
			});
			return this;
		}

		public FluentMapperConfigurator<TSource, TTarget> ForMember(
			Expression<Func<TTarget, object>> targetSelector,
			Expression<Func<TSource, object>> sourceSelector,
			Func<object, object> mapFunction)
		{
			_mappings.Add(new MappingConfiguration<TSource, TTarget>
			{
				SourceSelector = sourceSelector,
				TargetSelector = targetSelector,
				CustomMapFunction = mapFunction
			});
			return this;
		}

		public List<MappingConfiguration<TSource, TTarget>> Build() => _mappings;
	}

	public class MappingConfiguration<TSource, TTarget>
	{
		public Expression<Func<TSource, object>> SourceSelector { get; set; }
		public Expression<Func<TTarget, object>> TargetSelector { get; set; }
		public Func<object, object>? CustomMapFunction { get; set; }
	}

	public abstract class MappingProfile
	{
		protected MappingProfile(IMapperService mapperService)
		{
			ConfigureMappings(mapperService);
		}

		protected abstract void ConfigureMappings(IMapperService mapperService);
	}
}