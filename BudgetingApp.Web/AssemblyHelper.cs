namespace BudgetingApp.Web
{
    using System.Reflection;

    public static class AssemblyHelper
    {
        public static IEnumerable<Assembly> GetAssemblies(string namespacePrefix)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => assembly.FullName.StartsWith(namespacePrefix, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }
}