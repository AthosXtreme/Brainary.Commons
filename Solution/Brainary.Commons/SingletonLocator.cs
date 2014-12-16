namespace Brainary.Commons
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Base helper class to implement a singleton
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingletonLocator<T> : ILocator where T : SingletonLocator<T>
    {
        private static T instance;
        private ILocator locatorInstance;
        
        // ReSharper disable once EmptyConstructor
        protected SingletonLocator()
        {
        }

        /// <summary>
        /// Check if locator instance is already initialized
        /// </summary>
        public static bool IsReady
        {
            get
            {
                return Initialised && UniqueInstance.locatorInstance != null;
            }
        }

        protected static bool Initialised
        {
            get { return instance != null; }
        }

        protected static T UniqueInstance
        {
            get { return Initialised ? SingletonCreator.Instance : null; }
        }

        /// <summary>
        /// Component registration
        /// </summary>
        public virtual void RegisterComponents()
        {
            AssertInitialize();
            locatorInstance.RegisterComponents();
        }

        /// <summary>
        /// Obtain a default typed object instance
        /// </summary>
        /// <typeparam name="TU">Type expected</typeparam>
        /// <returns>Object</returns>
        public TU Resolve<TU>()
        {
            return Resolve<TU>(null);
        }

        /// <summary>
        /// Obtain a named and typed object instance
        /// </summary>
        /// <typeparam name="TU">Type expected</typeparam>
        /// <param name="name">Named instance</param>
        /// <returns>Object</returns>
        public TU Resolve<TU>(string name)
        {
            AssertInitialize();
            return locatorInstance.Resolve<TU>(name);
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
            UniqueInstance.locatorInstance.Dispose();
        }

        protected static void Init(T newInstance)
        {
            if (newInstance == null) throw new ArgumentNullException();
            instance = newInstance;
        }

        /// <summary>
        /// Must call this before use
        /// </summary>
        /// <param name="locator">Implemented container</param>
        protected void BaseInitialize(ILocator locator)
        {
            if (UniqueInstance.locatorInstance != null) throw new InvalidOperationException(Messages.AlreadyInitializedLocator);
            UniqueInstance.locatorInstance = locator;
        }

        protected void AssertInitialize()
        {
            if (locatorInstance == null) throw new InvalidOperationException(Messages.InitializeLocatorFirst);
        }

        private static class SingletonCreator
        {
            // ReSharper disable once StaticFieldInGenericType
            internal static readonly T Instance = instance;

            [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1409:RemoveUnnecessaryCode", Justification = "Reviewed. Suppression is OK here.")]
            static SingletonCreator()
            {
            }
        }
    }
}
