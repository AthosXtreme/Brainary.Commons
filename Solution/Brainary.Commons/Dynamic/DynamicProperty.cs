namespace Brainary.Commons.Dynamic
{
    using System;
    using System.Reflection.Emit;

    public class DynamicProperty
    {
        #region Fields

        private readonly string name;

        private readonly Type type;

        #endregion

        #region Constructors and Destructors

        public DynamicProperty(string name, Type type, CustomAttributeBuilder attributeBuilder = null)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            this.name = name;
            this.type = type;
            CustomAttributeBuilder = attributeBuilder;
        }

        #endregion

        #region Public Properties

        public CustomAttributeBuilder CustomAttributeBuilder { get; private set; }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public Type Type
        {
            get
            {
                return type;
            }
        }

        #endregion
    }
}