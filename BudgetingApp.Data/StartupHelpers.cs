using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BudgetingApp.Data
{
    public static class StartupHelpers
    {
        public static void AddDbContexts<TContext>(this IServiceCollection services, IConfiguration configuration, string configKey, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped) where TContext : DbContext
        {
            var connectionString = configuration.GetConnectionString(configKey);

            services.AddDbContext<BudgetingDbContext>(options =>
            {
                var serverVersion = ServerVersion.AutoDetect(connectionString);
                options.UseMySql(connectionString, serverVersion, o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)).EnableDetailedErrors();
#if DEBUG
                options.EnableSensitiveDataLogging();
#endif
            }, contextLifetime: ServiceLifetime.Transient, optionsLifetime: ServiceLifetime.Singleton);

            //services.AddDbContext<BudgetingDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
            //            services.AddDbContext<TContext>(options =>
            //            {
            //                options
            //                .UseSqlite(connectionString, o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
            //                .EnableDetailedErrors()
            //                .ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning));

            //#if DEBUG
            //                options.EnableSensitiveDataLogging();
            //#endif
            //            }
            //            , contextLifetime: serviceLifetime, optionsLifetime: ServiceLifetime.Singleton);
        }

        public static void AddDbContextsPooled<TContext>(this IServiceCollection services, IConfiguration configuration, string configKey) where TContext : DbContext
        {
            var connectionString = configuration.GetConnectionString(configKey);

            services.AddDbContextPool<TContext>(options => options.UseSqlite(connectionString));
        }
    }
}