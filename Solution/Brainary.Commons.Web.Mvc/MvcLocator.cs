namespace Brainary.Commons.Web.Mvc
{
    using System.Web.Mvc;

    /// <summary>
    /// Singleton MVC resolver implementation of <see cref="IMvcLocator"/>
    /// </summary>
    public sealed class MvcLocator : SingletonLocator<MvcLocator>, IMvcLocator
    {
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
                if (!Initialised)
                {
                    Init(new MvcLocator());
                }

                return UniqueInstance;
            }
        }
        #endregion
        
        /// <summary>
        /// Must call this before use
        /// </summary>
        /// <param name="locator">Implemented container</param>
        public static void Initialize(IMvcLocator locator)
        {
            var instance = (MvcLocator)Instance;
            instance.BaseInitialize(locator);
            instance.locatorInstance = locator;
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
    }
}
