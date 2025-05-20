using BudgetingApp.Web;
using BudgetingApp.Web.Services;
using System.Reflection;

// should be system
namespace System
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMapperly(this IServiceCollection services, params Assembly[] assemblies)
        {
            var mapperService = new MapperService();

            // Scan assemblies for mapper classes
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    // Get all the IMapperClass<> interfaces so we can add them to our service
                    var interfaces = type.GetInterfaces()
                        .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapperClass<,>));

                    foreach (var @interface in interfaces)
                    {
                        var sourceType = @interface.GetGenericArguments()[0];
                        var destinationType = @interface.GetGenericArguments()[1];

                        var mapperInstance = Activator.CreateInstance(type);

                        // Get the RegisterMapper method and invoke it with the generic types (as we don't know them at runtime)
                        var registerMethod = typeof(MapperService)
                                                .GetMethod(nameof(MapperService.RegisterMapper))
                                                .MakeGenericMethod(sourceType, destinationType);

                        registerMethod.Invoke(mapperService, new[] { mapperInstance });
                    }
                }
            }

            services.AddSingleton<IMapperService>(mapperService);

            return services;
        }
    }
}