using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Linq.Expressions;
using System.Reflection;

namespace BudgetingApp.Data
{
    public static class ContextExt
    {
        public static string QueryToSql<TEntity>(this IQueryable<TEntity> query) where TEntity : class
        {
            var enumerator = query.Provider.Execute<IEnumerable<TEntity>>(query.Expression).GetEnumerator();
            var relationalCommandCache = enumerator.Private("_relationalCommandCache");
            var selectExpression = relationalCommandCache.Private<SelectExpression>("_selectExpression");
            var factory = relationalCommandCache.Private<IQuerySqlGeneratorFactory>("_querySqlGeneratorFactory");

            var sqlGenerator = factory.Create();
            var command = sqlGenerator.GetCommand(selectExpression);

            string sql = command.CommandText;
            return sql;
        }

        private static object Private(this object obj, string privateField) => obj?.GetType().GetField(privateField, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);

        private static T Private<T>(this object obj, string privateField) => (T)obj?.GetType().GetField(privateField, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);

        public static void AddOrUpdate<T>(this DbContext context, T data, Func<T, int> primaryKey) where T : class
        {
            var id = primaryKey(data);

            var defaultType = default(int);

            if (id < defaultType)
            {
                // Skip as entity has a temporary id
                return;
            }

            context.Entry(data).State = (id == defaultType ? EntityState.Added : EntityState.Modified);
        }

        public static void AddOrUpdate<T>(this DbContext context, T data, Func<T, long> primaryKey, Action<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<T>> entityUpdate = null) where T : class
        {
            var id = primaryKey(data);

            var defaultTyep = default(long);

            if (id < defaultTyep)
            {
                //Skip as entity has a temporary id
                return;
            }

            context.Entry(data).State = (id == defaultTyep ? EntityState.Added : EntityState.Modified);

            if (entityUpdate != null)
            {
                entityUpdate(context.Entry(data));
            }
        }

        public static void AddOrUpdate<T>(this DbContext context, T data, Func<T, ulong> primaryKey) where T : class
        {
            var id = primaryKey(data);

            var defaultTyep = default(ulong);

            if (id < defaultTyep)
            {
                //Skip as entity has a temporary id
                return;
            }

            context.Entry(data).State = (id == defaultTyep ? EntityState.Added : EntityState.Modified);
        }

        public static void AddOrUpdate<T>(this DbContext context, T data, bool add = true) where T : class
        {
            context.Entry(data).State = add ? EntityState.Added : EntityState.Modified;
        }

        public static bool HasEntity<T>(this DbContext context, T data, Func<T, int> primaryKey) where T : class
        {
            var id = primaryKey(data);

            return context.Set<T>().Local.Any(f => primaryKey(f) == id);
        }

        public static bool HasEntity<T>(this DbContext context, T data, Func<T, long> primaryKey) where T : class
        {
            var id = primaryKey(data);

            return context.Set<T>().Local.Any(f => primaryKey(f) == id);
        }

        public static bool SaveChangesCheckConcurrency(this DbContext context, Func<bool> saveChanges)
        {
            bool saveFailed;
            var saveAttempts = 0; //attempts

            var result = false;

            do
            {
                saveFailed = false;

                result = saveChanges();

                try
                {
                    context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveAttempts++;

                    if (saveAttempts >= 20)
                    {
                        throw;
                    }

                    saveFailed = true;
                    ex.Entries.Single().Reload();
                }
            } while (saveFailed);

            return result;
        }

        public static bool HasBaseInterface(this Type type, Type baseInterface)
        {
            if (type.BaseType == null)
                return type.GetInterfaces().Contains(baseInterface);
            else
                return type.GetInterfaces().Except(type.BaseType.GetInterfaces()).Contains(baseInterface);
        }

        public static void OnBeforeSaving(this DbContext dbContext)
        {
            foreach (var entry in dbContext.ChangeTracker.Entries())
            {
                if (typeof(ISoftDelete).IsAssignableFrom(entry.Entity.GetType()))
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            entry.CurrentValues["IsDeleted"] = false;
                            break;

                        case EntityState.Deleted:
                            entry.State = EntityState.Modified;
                            entry.CurrentValues["IsDeleted"] = true;
                            break;
                    }
                }
            }
        }

        public static void AddIsDeleted(this ModelBuilder builder)
        {
            foreach (var entity in builder.Model.GetEntityTypes())
            {
                if (entity.ClrType.HasBaseInterface(typeof(ISoftDelete)))
                {
                    entity.AddProperty(_isDeletedProperty, typeof(bool));

                    builder
                        .Entity(entity.ClrType)
                        .HasQueryFilter(GetIsDeletedRestriction(entity.ClrType));

                    builder
                         .Entity(entity.ClrType)
                        .HasIndex(_isDeletedProperty)
                        .HasFilter($"{_isDeletedProperty} = 0");
                }
            }
        }

        private const string _isDeletedProperty = "IsDeleted";
        private static readonly MethodInfo _propertyMethod = typeof(EF).GetMethod(nameof(EF.Property), BindingFlags.Static | BindingFlags.Public).MakeGenericMethod(typeof(bool));

        private static LambdaExpression GetIsDeletedRestriction(Type type)
        {
            var parm = Expression.Parameter(type, "it");
            var prop = Expression.Call(_propertyMethod, parm, Expression.Constant(_isDeletedProperty));
            var condition = Expression.MakeBinary(ExpressionType.Equal, prop, Expression.Constant(false));
            var lambda = Expression.Lambda(condition, parm);
            return lambda;
        }

        public static void ClearChanges(this DbContext dbContext)
        {
            var changes = dbContext.ChangeTracker.Entries();

            if (changes?.Any() ?? false)
            {
                foreach (var item in changes)
                {
                    item.State = EntityState.Detached;
                }
            }
        }
    }
}