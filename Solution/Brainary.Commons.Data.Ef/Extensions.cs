using System.Reflection;
using Brainary.Commons.Data.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Brainary.Commons.Data
{
    public static class Extensions
    {
        /// <summary>
        /// Register all public types derived from base type from a given assembly 
        /// </summary>
        /// <typeparam name="T">Entity base type</typeparam>
        /// <param name="modelBuilder">Model builder instance</param>
        /// <param name="fromAssembly">Assembly to look for</param>
        public static void AutoEntity<T>(this ModelBuilder modelBuilder, Assembly fromAssembly)
        {
            var types = fromAssembly.GetExportedTypes().Where(w => w.IsSubclassOf(typeof(T)));
            var method = modelBuilder.GetType().GetMethods().First((w) => w.Name == "Entity" && w.IsGenericMethod);
            foreach (var t in types)
            {
                var generic = method.MakeGenericMethod(t);
                generic.Invoke(modelBuilder, null);
            }
        }

        public static void RemovePluralizingTableName(this ModelBuilder modelBuilder)
        {
            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            {
                entityType.SetTableName(entityType.DisplayName());
            }
        }

        public static void AddTableNamePrefix(this ModelBuilder modelBuilder, string prefix)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                entityType.SetTableName(prefix + ((IEntityType)(object)entityType).GetTableName());
            }
        }

        public static void UseShortNamePk(this ModelBuilder modelBuilder)
        {
            foreach (var (entityType, key) in modelBuilder.Model.GetEntityTypes().SelectMany(s => s.GetKeys().Where(w => w.IsPrimaryKey()).Select(e => (s, e))))
            {
                key.SetName("PK_" + entityType.DisplayName());
            }
        }

        public static void UseShortNameFk(this ModelBuilder modelBuilder)
        {
            foreach (var tuple in modelBuilder.Model.GetEntityTypes().SelectMany(entityType => entityType.GetForeignKeys()
            .GroupBy(k => k.PrincipalEntityType.DisplayName(), (k, v) => new { Key = k, Many = v.Count() > 1, Values = v })
            .SelectMany(group => group.Values.Select((s, i) => new { Index = i, Value = s }).Select(fk => (entityType, group, fk)))))
            {
                var entityType = tuple.entityType;
                var group = tuple.group;
                var fk = tuple.fk;
                string text = group.Many ? $"_{fk.Index:D2}" : string.Empty;
                RelationalForeignKeyExtensions.SetConstraintName(fk.Value, "FK_" + entityType.DisplayName() + "_" + fk.Value.PrincipalEntityType.DisplayName() + text);
            }
        }

        public static void UseShortNameIx(this ModelBuilder modelBuilder)
        {
            foreach (var (entityType, ix) in modelBuilder.Model.GetEntityTypes().SelectMany(entityType => entityType.GetIndexes().Select((s, i) => (entityType, new { Index = i, Value = s }))))
            {
                ix.Value.SetDatabaseName($"IX_{ix.Index + 1:D2}_{entityType.DisplayName()}");
            }
        }

        public static void ApplyCustomAttributes(this ModelBuilder modelBuilder)
        {
            foreach (var tuple in modelBuilder.Model.GetEntityTypes().SelectMany(entityType => entityType.GetProperties().Select(prop => (entityType, prop))))
            {
                var entityType = tuple.entityType;
                var prop = tuple.prop;
                var memberInfo = (MemberInfo?)prop.PropertyInfo ?? prop.FieldInfo;
                if (memberInfo != null)
                {
                    var customAttributes = Attribute.GetCustomAttributes(memberInfo, inherit: false);

                    var computedColumnSqlAttribute = customAttributes.OfType<ComputedColumnSqlAttribute>().FirstOrDefault();
                    if (computedColumnSqlAttribute != null)
                        RelationalPropertyExtensions.SetComputedColumnSql(prop, computedColumnSqlAttribute.Statement);

                    var defaultValueSqlAttribute = customAttributes.OfType<DefaultValueSqlAttribute>().FirstOrDefault();
                    if (defaultValueSqlAttribute != null)
                        RelationalPropertyExtensions.SetDefaultValueSql(prop, defaultValueSqlAttribute.Statement);
                }
            }
        }
    }
}