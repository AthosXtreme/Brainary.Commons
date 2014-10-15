namespace Brainary.Commons.Dynamic
{
    using System;

    public sealed class ParseException : Exception
    {
        #region Fields

        private readonly int position;

        #endregion

        #region Constructors and Destructors

        public ParseException(string message, int position)
            : base(message)
        {
            this.position = position;
        }

        #endregion

        #region Public Properties

        public int Position
        {
            get
            {
                return position;
            }
        }

        #endregion

        #region Public Methods and Operators

        public override string ToString()
        {
            return string.Format(Messages.ParseExceptionFormat, Message, position);
        }

        #endregion
    }
}