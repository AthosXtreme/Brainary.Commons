namespace Brainary.Commons
{
    using System;

    /// <summary>
    /// <see cref="ILocator"/> implementation
    /// </summary>
    public sealed class Locator : Singleton<Locator>, ILocator
    {
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
                if (!Initialised)
                {
                    Init(new Locator());
                }

                return UniqueInstance;
            }
        }
        #endregion

        /// <summary>
        /// Must call this before use
        /// </summary>
        /// <param name="locator">Implemented container</param>
        public static void Initialize(ILocator locator)
        {
            if (((Locator)Instance).locatorInstance != null) throw new InvalidOperationException(Messages.AlreadyInitializedLocator);
            ((Locator)Instance).locatorInstance = locator;
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
            ((Locator)Instance).locatorInstance.Dispose();
        }

        private void AssertInitialize()
        {
            if (locatorInstance == null) throw new InvalidOperationException(Messages.InitializeLocatorFirst);
        }
    }
}
