namespace Brainary.Commons.Web.Wcf
{
    /// <summary>
    /// Singleton wcf resolver implementation of <see cref="IWcfLocator"/>
    /// </summary>
    public sealed class WcfLocator : SingletonLocator<WcfLocator>, IWcfLocator
    {
        private IWcfLocator locatorInstance;

        #region "Singleton Implementation"
        // Deny constructor
        private WcfLocator()
        {
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static IWcfLocator Instance
        {
            get
            {
                if (!Initialised)
                {
                    Init(new WcfLocator());
                }

                return UniqueInstance;
            }
        }
        #endregion

        /// <summary>
        /// Must call this before use
        /// </summary>
        /// <param name="locator">Implemented container</param>
        public static void Initialize(IWcfLocator locator)
        {
            var instance = (WcfLocator)Instance;
            instance.BaseInitialize(locator);
            instance.locatorInstance = locator;
        }

        public void RegisterWcfComponents()
        {
            AssertInitialize();
            locatorInstance.RegisterWcfComponents();
        }
    }
}
