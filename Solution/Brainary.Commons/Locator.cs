namespace Brainary.Commons
{
    /// <summary>
    /// <see cref="SingletonLocator{T}"/> implementation
    /// </summary>
    public sealed class Locator : SingletonLocator<Locator>
    {
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
            ((Locator)Instance).BaseInitialize(locator);
        }
    }
}
