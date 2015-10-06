
namespace Serenity.Data
{
    /// <summary>
    ///   Used to define aliases like (T0) with ability to access fields.</summary>
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
        public static Alias<TFields> Alias<TFields>(this TFields fields, int alias)
            where TFields : RowFieldsBase
        {
            return new Alias<TFields>(fields, alias);
        }

        public static Alias<TFields> Alias<TFields>(this TFields fields, string alias)
            where TFields : RowFieldsBase
        {
            return new Alias<TFields>(fields, alias);
        }
    }
}