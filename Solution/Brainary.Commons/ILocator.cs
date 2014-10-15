namespace Brainary.Commons
{
    using System;

    /// <summary>
    /// Instance resolver interface
    /// </summary>
    public interface ILocator : IDisposable
    {
        /// <summary>
        /// Obtain a default typed object instance
        /// </summary>
        /// <typeparam name="T">Type expected</typeparam>
        /// <returns>Object</returns>
        T Resolve<T>();

        /// <summary>
        /// Obtain a named and typed object instance
        /// </summary>
        /// <typeparam name="T">Type expected</typeparam>
        /// <param name="name">Named instance</param>
        /// <returns>Object</returns>
        T Resolve<T>(string name);

        /// <summary>
        /// Obtain an object by type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        object Resolve(Type type);
    }
}
