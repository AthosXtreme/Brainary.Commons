namespace Brainary.Commons.Data
{
    using System;

    public abstract class DescriptionsCommand
    {
        protected DescriptionsCommand(string resultScriptFilePath, string columnDescriptionsTemplate)
        {
            if (resultScriptFilePath == null) throw new ArgumentNullException("resultScriptFilePath");
            if (columnDescriptionsTemplate == null) throw new ArgumentNullException("columnDescriptionsTemplate");

            ResultScriptFilePath = resultScriptFilePath;
            ColumnDescriptionsTemplate = columnDescriptionsTemplate;
        }

        public string ResultScriptFilePath { get; protected set; }

        public string ColumnDescriptionsTemplate { get; protected set; }

        public string EnumsReferenceTableScript { get; protected set; }

        public string EnumsReferenceTemplate { get; protected set; }
    }
}
