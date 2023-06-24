using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Brainary.Commons.Data.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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
            var entityMethod = modelBuilder.GetType().GetMethod("Entity", 1, Array.Empty<Type>());
            foreach (var t in types)
            {
                var entityGeneric = entityMethod?.MakeGenericMethod(t);
                var entityDelegate = (Func<EntityTypeBuilder>)Delegate.CreateDelegate(typeof(Func<EntityTypeBuilder>), modelBuilder, entityGeneric!);
                var entityTypeBuilder = entityDelegate();

                var options = t.GetTypeInfo().GetCustomAttribute<EntityOptionsAttribute>() ?? new EntityOptionsAttribute();
                var propertyMethod = entityTypeBuilder?.GetType().GetMethod("Property", 0, new Type[] { typeof(string) });
                var propertyDelegate = (Func<string, PropertyBuilder>)Delegate.CreateDelegate(typeof(Func<string, PropertyBuilder>), entityTypeBuilder, propertyMethod!);
                var propertyBuilder = propertyDelegate("Id");

                // Set MaxLength for array or string Id
                if (typeof(System.Collections.IEnumerable).IsAssignableFrom(propertyBuilder.Metadata.PropertyInfo!.PropertyType))
                {
                    var mlMethod = propertyBuilder?.GetType().GetMethod("HasMaxLength", 0, new Type[] { typeof(int) });
                    var mlDelegate = (Func<int, PropertyBuilder>)Delegate.CreateDelegate(typeof(Func<int, PropertyBuilder>), propertyBuilder, mlMethod!);
                    mlDelegate(options.MaxLengthId);
                }

                // Prevent identity Id when requested
                if (options.PreventIdentityId)
                {
                    var vgnMethod = propertyBuilder?.GetType().GetMethod("ValueGeneratedNever", 0, Array.Empty<Type>());
                    var vgnDelegate = (Func<PropertyBuilder>)Delegate.CreateDelegate(typeof(Func<PropertyBuilder>), propertyBuilder, vgnMethod!);
                    vgnDelegate();
                }
            }
        }

        /// <summary>
        /// Uses entity name as table name
        /// </summary>
        /// <param name="modelBuilder">Model builder instance</param>
        public static void RemovePluralizingTableName(this ModelBuilder modelBuilder)
        {
            var maxLen = modelBuilder.Model.GetMaxIdentifierLength();
            var types = modelBuilder.Model.GetEntityTypes();
            var subchars = 5;
            var digits = 2;
            foreach (IMutableEntityType entityType in types)
            {
                var idName = RegularTableName(entityType);
                var chsum = Checksum(idName, digits);
                var idnLen = idName.Length;
                idName = idnLen > maxLen ? $"{idName[..(maxLen - subchars)].TrimEnd('_')}_{chsum}{idnLen.ToString($"X{digits}")}" : idName;
                entityType.SetTableName(idName);
                entityType.SetAnnotation(nameof(RemovePluralizingTableName), true);
            }
        }

        /// <summary>
        /// Add a prefix to table names
        /// </summary>
        /// <param name="modelBuilder">Model builder instance</param>
        /// <param name="prefix">The prefix</param>
        public static void AddTableNamePrefix(this ModelBuilder modelBuilder, string prefix)
        {
            var maxLen = modelBuilder.Model.GetMaxIdentifierLength();
            var types = modelBuilder.Model.GetEntityTypes();
            var subchars = 5;
            var digits = 2;
            foreach (var entityType in types)
            {
                var idName = ((bool?)entityType.FindAnnotation(nameof(RemovePluralizingTableName))?.Value ?? false) ? $"{prefix}{RegularTableName(entityType)}" : $"{prefix}{entityType.GetTableName()}";
                var chsum = Checksum(idName, digits);
                var idnLen = idName.Length;
                idName = idnLen > maxLen ? $"{idName[..(maxLen - subchars)].TrimEnd('_')}_{chsum}{idnLen.ToString($"X{digits}")}" : idName;
                entityType.SetTableName(idName);
                entityType.SetAnnotation(nameof(AddTableNamePrefix), true);
            }
        }

        /// <summary>
        /// Set short PK names
        /// </summary>
        /// <param name="modelBuilder">Model builder instance</param>
        public static void UseShortNamePk(this ModelBuilder modelBuilder)
        {
            var maxLen = modelBuilder.Model.GetMaxIdentifierLength();
            var types = modelBuilder.Model.GetEntityTypes().SelectMany(s => s.GetKeys().Where(w => w.IsPrimaryKey()).Select(e => (s, e)));
            var subchars = 5;
            var digits = 2;
            foreach (var (entityType, key) in types)
            {
                var tableName = RegularTableName(entityType);
                var idName = $"PK_{tableName}";
                var chsum = Checksum(idName, digits);
                var idnLen = idName.Length;
                idName = idnLen > maxLen ? $"{idName[..(maxLen - subchars)].TrimEnd('_')}_{chsum}{idnLen.ToString($"X{digits}")}" : idName;
                key.SetName(idName);
            }
        }

        /// <summary>
        /// Set short FK names
        /// </summary>
        /// <param name="modelBuilder">Model builder instance</param>
        public static void UseShortNameFk(this ModelBuilder modelBuilder)
        {
            var maxLen = modelBuilder.Model.GetMaxIdentifierLength();
            var types = modelBuilder.Model.GetEntityTypes().SelectMany(et => et.GetForeignKeys().Select(fk => (et, fk)));
            var subchars = 5;
            var digits = 2;
            foreach (var (entityType, fk) in types)
            {
                if (entityType.IsPropertyBag) // Correct implicitly created entity column names
                {
                    foreach (var property in fk.Properties)
                    {
                        var principal = property.FindFirstPrincipal()!;
                        var fkColumnName = $"{principal.DeclaringEntityType.DisplayName()}{principal.Name}";
                        property.SetColumnName(fkColumnName);
                    }
                }
                var entityTableName = RegularTableName(entityType);
                var fkTableName = RegularTableName(fk.PrincipalEntityType);
                var fkColumnNames = string.Join("_", fk.Properties.Select(s => s.GetColumnName()));
                var idName = $"FK_{entityTableName}_{fkTableName}";
                var chsum = Checksum(fkColumnNames, digits);
                idName = (idName.Length + subchars) > maxLen ? $"{idName[..(maxLen - subchars)]}".TrimEnd('_') : idName;
                idName += $"_{chsum}{fkColumnNames.Length.ToString($"X{digits}")}";
                RelationalForeignKeyExtensions.SetConstraintName(fk, idName);
            }
        }

        /// <summary>
        /// Set short IX names
        /// </summary>
        /// <param name="modelBuilder">Model builder instance</param>
        public static void UseShortNameIx(this ModelBuilder modelBuilder)
        {
            var maxLen = modelBuilder.Model.GetMaxIdentifierLength();
            var types = modelBuilder.Model.GetEntityTypes().SelectMany(entityType => entityType.GetIndexes().Select(ix => (entityType, ix)));
            var subchars = 5;
            var digits = 2;
            foreach (var (entityType, ix) in types)
            {
                var tableName = RegularTableName(entityType);
                var ixColumnNames = string.Join("_", ix.Properties.Select(s => s.GetColumnName()));
                var chsum = Checksum(ixColumnNames, digits);
                var idName = $"IX_{tableName}";
                idName = (idName.Length + subchars) > maxLen ? $"{idName[..(maxLen - subchars)]}".TrimEnd('_') : idName;
                idName += $"_{chsum}{ixColumnNames.Length.ToString($"X{digits}")}";
                ix.SetDatabaseName(idName);
            }
        }

        /// <summary>
        /// Set string value for enums
        /// </summary>
        /// <param name="modelBuilder">Model builder instance</param>
        public static void UseStringEnumValues(this ModelBuilder modelBuilder)
        {
            foreach (var (prop, enumType) in modelBuilder.Model.GetEntityTypes().SelectMany(entityType => entityType.GetProperties().Select(s => (s, Nullable.GetUnderlyingType(s.ClrType) ?? s.ClrType))).Where(w => w.Item2.IsEnum))
            {
                var converterType = typeof(EnumToStringConverter<>).MakeGenericType(enumType);
                prop.SetValueConverter((ValueConverter)Activator.CreateInstance(converterType)!);
            }
        }

        /// <summary>
        /// Applies known custom atrtibutes for model
        /// </summary>
        /// <param name="modelBuilder">Model builder instance</param>
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

                    var decimalPrecisionAttribute = customAttributes.OfType<DecimalPrecisionAttribute>().FirstOrDefault();
                    if (decimalPrecisionAttribute != null)
                    {
                        prop.SetPrecision(decimalPrecisionAttribute.Precision);
                        prop.SetScale(decimalPrecisionAttribute.Scale);
                    }
                }
            }
        }
        private static string RegularTableName(IMutableEntityType entityType)
        {
            var attr = entityType.ClrType.GetTypeInfo().GetCustomAttribute<TableAttribute>();
            var idName = attr != null && !string.IsNullOrWhiteSpace(attr.Name) ? attr.Name : entityType.ShortName();
            return idName;
        }

        private static string Checksum(string value, int digits)
        {
            return value.Aggregate(0, (p, v) => p ^ v).ToString($"X{digits}");
        }
    }
}