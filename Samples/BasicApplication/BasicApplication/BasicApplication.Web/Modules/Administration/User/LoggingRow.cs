namespace BasicApplication.Administration.Entities
{
    using Serenity.ComponentModel;
    using Serenity.Data;
    using Serenity.Data.Mapping;
    using System;

    public abstract class LoggingRow : Row, IIsActiveRow, ILoggingRow
    {
        protected LoggingRow(RowFieldsBase fields)
            : base(fields)
        {
            loggingFields = (LoggingRowFields)fields;
        }

        [NotNull, Insertable(false), Updatable(false)]
        public Int32? InsertUserId
        {
            get { return loggingFields.InsertUserId[this]; }
            set { loggingFields.InsertUserId[this] = value; }
        }

        [NotNull, Insertable(false), Updatable(false)]
        public DateTime? InsertDate
        {
            get { return loggingFields.InsertDate[this]; }
            set { loggingFields.InsertDate[this] = value; }
        }

        [Insertable(false), Updatable(false)]
        public Int32? UpdateUserId
        {
            get { return loggingFields.UpdateUserId[this]; }
            set { loggingFields.UpdateUserId[this] = value; }
        }

        [Insertable(false), Updatable(false)]
        public DateTime? UpdateDate
        {
            get { return loggingFields.UpdateDate[this]; }
            set { loggingFields.UpdateDate[this] = value; }
        }

        [NotNull, Insertable(false), Updatable(true)]
        public Int16? IsActive
        {
            get { return loggingFields.IsActive[this]; }
            set { loggingFields.IsActive[this] = value; }
        }

        Int16Field IIsActiveRow.IsActiveField
        {
            get { return loggingFields.IsActive; }
        }

        IIdField IInsertLogRow.InsertUserIdField
        {
            get { return loggingFields.InsertUserId; }
        }

        IIdField IUpdateLogRow.UpdateUserIdField
        {
            get { return loggingFields.UpdateUserId; }
        }

        DateTimeField IInsertLogRow.InsertDateField
        {
            get { return loggingFields.InsertDate; }
        }

        DateTimeField IUpdateLogRow.UpdateDateField
        {
            get { return loggingFields.UpdateDate; }
        }

        private LoggingRowFields loggingFields;

        public class LoggingRowFields : RowFieldsBase
        {
            public readonly Int32Field InsertUserId;
            public readonly DateTimeField InsertDate;
            public readonly Int32Field UpdateUserId;
            public readonly DateTimeField UpdateDate;
            public readonly Int16Field IsActive;

            public LoggingRowFields(string tableName)
                : base(tableName)
            {
            }
        }
    }
}