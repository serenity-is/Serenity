using Serenity.Data;
using Serenity.Data.Mapping;
using System;
using System.ComponentModel;

namespace Serenity.Test.Logging
{
    public partial class SqlLoggerTests
    {
        [TableName("SystemLogs")]
        public class SystemLogRow : Row
        {
            [Identity]
            public Int32? ID
            {
                get { return Fields.ID[this]; }
                set { Fields.ID[this] = value; }
            }

            public DateTime? EventDate
            {
                get { return Fields.EventDate[this]; }
                set { Fields.EventDate[this] = value; }
            }

            [Size(10)]
            public String LogLevel
            {
                get { return Fields.LogLevel[this]; }
                set { Fields.LogLevel[this] = value; }
            }

            public String LogMessage
            {
                get { return Fields.LogMessage[this]; }
                set { Fields.LogMessage[this] = value; }
            }

            public String Exception
            {
                get { return Fields.Exception[this]; }
                set { Fields.Exception[this] = value; }
            }

            [Size(200)]
            public String SourceType
            {
                get { return Fields.SourceType[this]; }
                set { Fields.SourceType[this] = value; }
            }

            public class RowFields : RowFieldsBase
            {
                public Int32Field ID;
                public DateTimeField EventDate;
                public StringField LogLevel;
                public StringField LogMessage;
                public StringField Exception;
                public StringField SourceType;

                public RowFields()
                {
                }
            }

            public static RowFields Fields = new RowFields().Init();

            public SystemLogRow()
                : base(Fields)
            {
            }
        }
    }
}