namespace Brainary.Commons.Helpers
{
    using System;
    using System.Configuration;
    using System.Linq;

    /// <summary>
    /// Configuration helper methods
    /// </summary>
    public static class ConfigHelper
    {
        /// <summary>
        /// Get connection string by name
        /// </summary>
        /// <param name="name">Connection string name</param>
        /// <returns>Connection string</returns>
        public static string GetConnectionString(string name)
        {
            if (ConfigurationManager.ConnectionStrings.Cast<ConnectionStringSettings>().All(a => a.Name != name))
                throw new InvalidOperationException(string.Format(Messages.ConnectionStringNotFound, name));

            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }

        /// <summary>
        /// Get app setting value by key
        /// </summary>
        /// <param name="key">App setting key</param>
        /// <returns>String value</returns>
        public static string GetSetting(string key)
        {
            if (!ConfigurationManager.AppSettings.AllKeys.Contains(key))
                throw new InvalidOperationException(string.Format(Messages.AppSettingNotFound, key));

            return ConfigurationManager.AppSettings[key];
        }

        /// <summary>
        /// Get typed app setting value by key
        /// </summary>
        /// <typeparam name="T">Type expected</typeparam>
        /// <param name="key">App setting key</param>
        /// <returns>Object</returns>
        public static T GetSetting<T>(string key) where T : struct
        {
            var strValue = GetSetting(key);
            var typeCode = Convert.GetTypeCode(new T());
            var objValue = Convert.ChangeType(strValue, typeCode);
            if (objValue == null) return default(T);
            return (T)objValue;
        }
    }
}