namespace Brainary.Commons.Web
{
    using System;
    using System.Web.Mvc;

    /// <summary>
    /// Singleton global resolver implementation of <see cref="ILocator"/>
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

        public static void Initialize(ILocator locator)
        {
            if (((Locator)Instance).locatorInstance != null) throw new InvalidOperationException(Messages.AlreadyInitializedLocator);
            ((Locator)Instance).locatorInstance = locator;

            // initialize base locator too
            Commons.Locator.Initialize(locator);
        }

        public T Resolve<T>()
        {
            return Resolve<T>(null);
        }

        public T Resolve<T>(string name)
        {
            AssertInitialize();
            return locatorInstance.Resolve<T>(name);
        }

        public object Resolve(Type type)
        {
            AssertInitialize();
            return locatorInstance.Resolve(type);
        }

        public void RegisterMvcComponents()
        {
            AssertInitialize();
            locatorInstance.RegisterMvcComponents();
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

        public void RegisterWcfComponents()
        {
            AssertInitialize();
            locatorInstance.RegisterWcfComponents();
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
