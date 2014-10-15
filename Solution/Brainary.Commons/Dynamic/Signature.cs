namespace Brainary.Commons.Dynamic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class Signature : IEquatable<Signature>
    {
        #region Constructors and Destructors

        public Signature(IEnumerable<DynamicProperty> properties)
        {
            Properties = properties.ToArray();
            HashCode = 0;
            foreach (DynamicProperty p in properties)
            {
                HashCode ^= p.Name.GetHashCode() ^ p.Type.GetHashCode();
            }
        }

        #endregion

        #region Public Properties

        public int HashCode { get; set; }

        public DynamicProperty[] Properties { get; set; }

        #endregion

        #region Public Methods and Operators

        public override bool Equals(object obj)
        {
            return obj is Signature && Equals((Signature)obj);
        }

        public bool Equals(Signature other)
        {
            if (Properties.Length != other.Properties.Length)
            {
                return false;
            }

            return
                !Properties.Where((t, i) => t.Name != other.Properties[i].Name || t.Type != other.Properties[i].Type)
                     .Any();
        }

        public override int GetHashCode()
        {
            return HashCode;
        }

        #endregion
    }
}