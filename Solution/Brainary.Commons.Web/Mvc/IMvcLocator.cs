namespace Brainary.Commons.Web.Mvc
{
    using System.Web.Mvc;

    /// <summary>
    /// Extends <see cref="ILocator"/> interface for MVC implementations
    /// </summary>
    public interface IMvcLocator : ILocator
    {
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
    }
}
