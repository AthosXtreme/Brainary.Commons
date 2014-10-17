namespace Brainary.Commons.Data
{
    public class MsSqlDescriptionsCommand : DescriptionsCommand
    {
        private const string ScriptFilePath = @"..\..\..\..\Scripts\descriptions_script.sql";

        private const string ColDescTempl =
            "IF NOT EXISTS (SELECT NULL FROM SYS.EXTENDED_PROPERTIES WHERE [major_id] = OBJECT_ID('{0}') AND [name] = N'MS_Description' AND [minor_id] = (SELECT [column_id] FROM SYS.COLUMNS WHERE [name] = '{1}' AND [object_id] = OBJECT_ID('{0}')))\r\n"
            + "\tEXEC sys.sp_addextendedproperty    @name='MS_Description', @value=N'{2}', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'{0}', @level2type=N'COLUMN', @level2name=N'{1}'\r\n"
            + "ELSE\r\n"
            + "\tEXEC sys.sp_updateextendedproperty @name='MS_Description', @value=N'{2}', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'{0}', @level2type=N'COLUMN', @level2name=N'{1}'\r\n";

        private const string EnumsTblScript =
            "IF NOT EXISTS (SELECT NULL FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = '__EnumsReference')\r\n"
            + "\tCREATE TABLE [dbo].[__EnumsReference] (\r\n"
            + "\t\t[EnumsReferenceId] [int] NOT NULL IDENTITY,\r\n"
            + "\t\t[TypeName] [nvarchar](50) NOT NULL,\r\n"
            + "\t\t[EnumName] [nvarchar](50) NOT NULL,\r\n"
            + "\t\t[EnumDescription] [nvarchar](50) NOT NULL,\r\n"
            + "\t\t[EnumValue] [int] NOT NULL,\r\n"
            + "\t\tCONSTRAINT [PK_dbo.__EnumsReference] PRIMARY KEY ([EnumsReferenceId])\r\n"
            + "\t) ELSE TRUNCATE TABLE [__EnumsReference]\r\n";

        private const string EnumsRefTempl = "INSERT [__EnumsReference] ([TypeName], [EnumName], [EnumDescription], [EnumValue]) VALUES ('{0}', '{1}', '{2}', {3})";

        public MsSqlDescriptionsCommand()
            : base(ScriptFilePath, ColDescTempl)
        {
            EnumsReferenceTableScript = EnumsTblScript;
            EnumsReferenceTemplate = EnumsRefTempl;
        }

        public MsSqlDescriptionsCommand(string resultScriptFilePath)
            : base(resultScriptFilePath, ColDescTempl)
        {
            EnumsReferenceTableScript = EnumsTblScript;
            EnumsReferenceTemplate = EnumsRefTempl;
        }
    }
}
