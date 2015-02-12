namespace Brainary.Commons
{
    using System;

    /// <summary>
    /// Singleton resolver implementation of <see cref="ILocator"/>
    /// </summary>
    public sealed class Locator : ILocator
    {
        private static readonly Lazy<Locator> LazyInstance = new Lazy<Locator>(() => new Locator());

        private ILocator locatorInstance;

        #region "Singleton Implementation"
        // Deny constructor
        private Locator()
        {
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static ILocator Instance
        {
            get
            {
                return LazyInstance.Value;
            }
        }
        #endregion

        /// <summary>
        /// Must call this before use
        /// </summary>
        /// <param name="locator">Implemented container</param>
        public static void Initialize(ILocator locator)
        {
            var instance = (Locator)Instance;
            if (instance.locatorInstance != null) throw new InvalidOperationException(Messages.AlreadyInitializedLocator);
            instance.locatorInstance = locator;
        }

        /// <summary>
        /// Component registration
        /// </summary>
        public void RegisterComponents()
        {
            AssertInitialize();
            locatorInstance.RegisterComponents();
        }

        /// <summary>
        /// Obtain a default typed object instance
        /// </summary>
        /// <typeparam name="T">Type expected</typeparam>
        /// <returns>Object</returns>
        public T Resolve<T>()
        {
            return Resolve<T>(null);
        }

        /// <summary>
        /// Obtain a named and typed object instance
        /// </summary>
        /// <typeparam name="T">Type expected</typeparam>
        /// <param name="name">Named instance</param>
        /// <returns>Object</returns>
        public T Resolve<T>(string name)
        {
            AssertInitialize();
            return locatorInstance.Resolve<T>(name);
        }

        /// <summary>
        /// Obtain an object by type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public object Resolve(Type type)
        {
            AssertInitialize();
            return locatorInstance.Resolve(type);
        }

        public void Dispose()
        {
            locatorInstance.Dispose();
        }

        private void AssertInitialize()
        {
            if (locatorInstance == null) throw new InvalidOperationException(Messages.InitializeLocatorFirst);
        }
    }
}
