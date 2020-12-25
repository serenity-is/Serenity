using System;

namespace Serenity.Data
{
    public static class AliasedFields
    {
        public static TFields As<TFields>(this TFields fields, string alias)
            where TFields : RowFieldsBase
        {
            if (string.IsNullOrWhiteSpace(alias))
                throw new ArgumentNullException(nameof(alias));

            if (alias == fields.AliasName)
                return fields;

            return (TFields)RowFieldsProvider.Current.ResolveWithAlias(typeof(TFields), alias);
        }
    }
}