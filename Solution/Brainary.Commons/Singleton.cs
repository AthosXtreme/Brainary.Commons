namespace Brainary.Commons
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Base helper class to implement a singleton
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> where T : Singleton<T>
    {
        private static T instance;

        // ReSharper disable once EmptyConstructor
        protected Singleton()
        {
        }

        protected static bool Initialised
        {
            get { return instance != null; }
        }

        protected static T UniqueInstance
        {
            get { return Initialised ? SingletonCreator.Instance : null; }
        }

        protected static void Init(T newInstance)
        {
            if (newInstance == null) throw new ArgumentNullException();

            instance = newInstance;
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
