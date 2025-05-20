namespace BudgetingApp.Web.Services
{
	public class MapperService : IMapperService
	{
		private readonly List<object> _mapperRegistrations;

		public MapperService()
		{
			_mapperRegistrations = new List<object>();
		}

		public void RegisterMapper<TSource, TDestination>(IMapperClass<TSource, TDestination> mapper) where TSource : class where TDestination : class
		{
			var registration = new MapperRegistration<TSource, TDestination>(mapper);
			_mapperRegistrations.Add(registration);
		}

		public TDestination Map<TSource, TDestination>(TSource source) where TSource : class where TDestination : class
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			var sourceType = source.GetType();
			var destinationType = typeof(TDestination);

			var registration = _mapperRegistrations
				.OfType<MapperRegistration<TSource, TDestination>>() // Ensure correct types
				.FirstOrDefault(r => r.SourceType == sourceType);

			if (registration == null)
			{
				throw new InvalidOperationException($"No mapper found for {sourceType.Name} to {destinationType.Name}");
			}

			return registration.Mapper.MapTo((dynamic)source);
		}

		public ICollection<TDestination> MapList<TSource, TDestination>(ICollection<TSource> sourceList) where TSource : class where TDestination : class
		{
			if (sourceList == null) throw new ArgumentNullException(nameof(sourceList));

			if (!sourceList.Any()) return Array.Empty<TDestination>();

			var sourceType = typeof(TSource);
			var destinationType = typeof(TDestination);

			var registration = _mapperRegistrations
									.OfType<MapperRegistration<TSource, TDestination>>()
									.FirstOrDefault(r => r.SourceType == sourceType);

			if (registration == null)
			{
				throw new InvalidOperationException($"No mapper found for {sourceType.Name} to {destinationType.Name}");
			}

			return registration.Mapper.MapToList((dynamic)sourceList);
		}

		/// <summary>
		/// Merge source into target
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TDestination"></typeparam>
		/// <param name="source"></param>
		/// <param name="target"></param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public void Map<TSource, TDestination>(TSource source, TDestination target) where TSource : class where TDestination : class
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			var sourceType = source.GetType();
			var destinationType = typeof(TDestination);

			var registration = _mapperRegistrations
				.OfType<MapperRegistration<TSource, TDestination>>() // Ensure correct types
				.FirstOrDefault(r => r.SourceType == sourceType);

			if (registration == null)
			{
				throw new InvalidOperationException($"No mapper found for {sourceType.Name} to {destinationType.Name}");
			}

			registration.Mapper.Merge((dynamic)source, (dynamic)target);
		}
	}

	public class MapperRegistration<TSource, TDestination> where TSource : class where TDestination : class
	{
		public Type SourceType => typeof(TSource);
		public Type DestinationType => typeof(TDestination);
		public IMapperClass<TSource, TDestination> Mapper { get; }

		public MapperRegistration(IMapperClass<TSource, TDestination> mapper)
		{
			Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}
	}
}