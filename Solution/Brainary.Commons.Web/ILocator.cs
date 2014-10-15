namespace Brainary.Commons.Web
{
    using System.Web.Mvc;

    /// <summary>
    /// Extends <see cref="Commons.ILocator"/> interface for web implementations
    /// </summary>
    public interface ILocator : Commons.ILocator
    {
        /// <summary>
        /// Register MVC components
        /// </summary>
        void RegisterMvcComponents();

        /// <summary>
        /// Return filter provider
        /// </summary>
        /// <returns>Filter provider instance</returns>
        IFilterProvider GetMvcFilterProvider();

        /// <summary>
        /// Return model binder provider
        /// </summary>
        /// <returns>Model binder provider instance</returns>
        IModelBinderProvider GetMvcModelBinderProvider();

        /// <summary>
        /// Return dependency resolver
        /// </summary>
        /// <returns>Dependency resolver instance</returns>
        IDependencyResolver GetMvcDependencyResolver();

        /// <summary>
        /// Register WCF components
        /// </summary>
        void RegisterWcfComponents();
    }
}
