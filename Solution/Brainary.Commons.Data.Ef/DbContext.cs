namespace Brainary.Commons.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.Entity;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Brainary.Commons.Extensions;

    public abstract class DbContext : System.Data.Entity.DbContext
    {
        protected DbContext()
            : this(null)
        {
        }

        protected DbContext(DescriptionsCommand descriptionsCommand)
        {
            TableNameFormat = "{0}";
            EntityTypes = new List<Type>();
            DescriptionsCommand = descriptionsCommand;
        }

        protected static string TableNameFormat { get; set; }

        protected IList<Type> EntityTypes { get; private set; }

        protected DescriptionsCommand DescriptionsCommand { get; set; }

        public static string GetTableName(string name)
        {
            return string.Format(TableNameFormat, name);
        }

        public void SetDomainDescriptions()
        {
            if (DescriptionsCommand == null)
                throw new InvalidOperationException(Messages.DescriptionsCommandNotSet);

            var writer = File.CreateText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DescriptionsCommand.Path));

            foreach (var script in EntityTypes.SelectMany(
                etype => (from pinfo in etype.GetProperties() 
                          let attribute = pinfo.GetCustomAttribute<DescriptionAttribute>() 
                          where IsField(pinfo) && attribute != null 
                          select string.Format(DescriptionsCommand.Template, GetTableName(etype.Name), pinfo.Name, attribute.Description))))
            {
                writer.WriteLine(script);
                writer.Flush();
                DescriptionsCommand.Execute(this, script);
            }

            writer.Close();
            writer.Dispose();
        }

        protected void AddEntity<T>()
        {
            EntityTypes.Add(typeof(T));
        }

        protected void AddEntities<T>(Assembly fromAssembly)
        {
            var types = fromAssembly.GetExportedTypes().Where(w => w.IsSubclassOf(typeof(T))).ToList();
            EntityTypes = EntityTypes.Union(types).ToList();
        }

        protected void SetEntities(DbModelBuilder modelBuilder)
        {
            foreach (var generic in from type in EntityTypes let method = modelBuilder.GetType().GetMethod("Entity") select method.MakeGenericMethod(type))
                generic.Invoke(modelBuilder, null);
        }

        private static bool IsField(PropertyInfo pinfo)
        {
            return pinfo.PropertyType.IsPrimitive ||
                pinfo.PropertyType.IsNullablePrimitive() ||
                pinfo.PropertyType.IsEnum ||
                pinfo.PropertyType.IsNullableEnum() ||
                pinfo.PropertyType == typeof(DateTime) ||
                pinfo.PropertyType == typeof(DateTime?) ||
                pinfo.PropertyType == typeof(decimal) ||
                pinfo.PropertyType == typeof(decimal?) ||
                pinfo.PropertyType == typeof(string);
        }
    }
}
