namespace Brainary.Commons.Dynamic
{
    using System.Linq.Expressions;

    internal class DynamicOrdering
    {
        #region Public Properties

        public bool Ascending { get; set; }

        public Expression Selector { get; set; }

        #endregion
    }
}