
using System;

namespace Serenity.Data
{
    [Obsolete("Use Fields.As()")]
    public class Alias<TFields> : Alias
        where TFields: RowFieldsBase
    {
        private TFields field;

        public Alias(TFields fields, int alias)
            : base(fields.TableName, alias)
        {
            this.field = fields;
        }

        public Alias(TFields fields, string alias)
            : base(fields.TableName, alias)
        {
            this.field = fields;
        }

        public TFields Field
        {
            get { return field; }
        }
    }

    public static class AliasTFieldExtensions
    {
        [Obsolete("Use Fields.As()")]
        public static Alias<TFields> Alias<TFields>(this TFields fields, int alias)
            where TFields : RowFieldsBase
        {
            return new Alias<TFields>(fields, alias);
        }

        [Obsolete("Use Fields.As()")]
        public static Alias<TFields> Alias<TFields>(this TFields fields, string alias)
            where TFields : RowFieldsBase
        {
            return new Alias<TFields>(fields, alias);
        }
    }
}