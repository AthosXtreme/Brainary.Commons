namespace Brainary.Commons.Data
{
    using System;

    public abstract class DescriptionsCommand
    {
        protected DescriptionsCommand(string scriptTemplate, string relativeScriptPath)
        {
            if (scriptTemplate == null) throw new ArgumentNullException("scriptTemplate");
            if (relativeScriptPath == null) throw new ArgumentNullException("relativeScriptPath");

            Template = scriptTemplate;
            Path = relativeScriptPath;
        }

        public string Template { get; private set; }

        public string Path { get; private set; }

        public virtual void Execute(DbContext context, string script)
        {
            context.Database.ExecuteSqlCommand(script);
        }
    }
}
