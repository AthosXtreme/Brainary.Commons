﻿namespace Brainary.Commons.Web
{
    using System.Web.Mvc;

    /// <summary>
    /// Singleton global resolver implementation of <see cref="ILocator"/>
    /// </summary>
    public sealed class Locator : SingletonLocator<Locator>, ILocator
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
            var instance = (Locator)Instance;
            instance.BaseInitialize(locator);
            instance.locatorInstance = locator;
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
    }
}
