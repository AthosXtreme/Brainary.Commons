namespace Brainary.Commons.Web
{
    using Brainary.Commons.Web.Mvc;
    using Brainary.Commons.Web.Wcf;

    /// <summary>
    /// Extends all locators interfaces for wide implementations
    /// </summary>
    public interface ILocator : IMvcLocator, IWcfLocator, Commons.ILocator
    {
    }
}
