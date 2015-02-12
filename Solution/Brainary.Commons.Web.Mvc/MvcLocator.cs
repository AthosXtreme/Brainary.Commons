namespace Brainary.Commons.Web.Mvc
{
    using System;
    using System.Web.Mvc;

    /// <summary>
    /// Singleton MVC resolver implementation of <see cref="IMvcLocator"/>
    /// </summary>
    public sealed class MvcLocator : IMvcLocator
    {
        private static readonly Lazy<MvcLocator> LazyInstance = new Lazy<MvcLocator>(() => new MvcLocator());

        private IMvcLocator locatorInstance;

        #region "Singleton Implementation"
        // Deny constructor
        private MvcLocator()
        {
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static IMvcLocator Instance
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
        public static void Initialize(IMvcLocator locator)
        {
            Locator.Initialize(locator);
            var instance = (MvcLocator)Instance;
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

        public IFilterProvider GetMvcFilterProvider()
        {
            AssertInitialize();
            return locatorInstance.GetMvcFilterProvider();
        }

        public IModelBinderProvider GetMvcModelBinderProvider()
        {
            AssertInitialize();
            return locatorInstance.GetMvcModelBinderProvider();
        }

        public IDependencyResolver GetMvcDependencyResolver()
        {
            AssertInitialize();
            return locatorInstance.GetMvcDependencyResolver();
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
