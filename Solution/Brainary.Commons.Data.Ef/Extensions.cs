namespace Brainary.Commons.Data
{
    using System.Data.Entity;
    using System.Linq;
    using System.Reflection;

    public static class Extensions
    {
        /// <summary>
        /// Register all public types derived from base type from a given assembly 
        /// </summary>
        /// <typeparam name="T">Base type</typeparam>
        /// <param name="modelBuilder">Model builder instance</param>
        /// <param name="fromAssembly">Assembly to look for</param>
        public static void AutoEntity<T>(this DbModelBuilder modelBuilder, Assembly fromAssembly)
        {
            var types = fromAssembly.GetExportedTypes().Where(w => w.IsSubclassOf(typeof(T)));

            foreach (var generic in from type in types let method = modelBuilder.GetType().GetMethod("Entity") select method.MakeGenericMethod(type))
                generic.Invoke(modelBuilder, null);
        }
    }
}
