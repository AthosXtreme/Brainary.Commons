namespace Brainary.Commons.Unity
{
    using System;

    using Brainary.Commons;

    using Microsoft.Practices.Unity;

    public abstract class UnityLocator : ILocator
    {
        public IUnityContainer Container { get; set; }
        
        /// <summary>
        /// Component registration
        /// </summary>
        public abstract void RegisterComponents();
        
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
            return Container.Resolve<T>(name);
        }

        /// <summary>
        /// Obtain an object by type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public object Resolve(Type type)
        {
            return Container.Resolve(type);
        }

        public void Dispose()
        {
            Container.Dispose();
        }
    }
}
