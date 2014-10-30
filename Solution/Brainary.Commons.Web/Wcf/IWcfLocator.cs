namespace Brainary.Commons.Web.Wcf
{
    /// <summary>
    /// Extends <see cref="Commons.ILocator"/> interface for wcf implementations
    /// </summary>
    public interface IWcfLocator : Commons.ILocator
    {
        /// <summary>
        /// Register WCF components
        /// </summary>
        void RegisterWcfComponents();
    }
}
