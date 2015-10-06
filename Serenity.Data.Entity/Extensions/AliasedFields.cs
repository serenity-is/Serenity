using System;
using System.Collections.Concurrent;

namespace Serenity.Data
{
    public static class AliasedFields
    {
        private static readonly ConcurrentDictionary<Tuple<Type, string>, RowFieldsBase> cache =
            new ConcurrentDictionary<Tuple<Type, string>, RowFieldsBase>();

        public static TFields As<TFields>(this TFields fields, string alias)
            where TFields : RowFieldsBase, new()
        {
            Check.NotNullOrEmpty(alias, "alias");
            if ((alias == "t0" || alias == "T0") &&
                (fields.alias == "t0" || fields.alias == "T0"))
            {
                return fields;
            }

            return Get<TFields>(alias);
        }

        public static TFields Get<TFields>(string alias)
            where TFields: RowFieldsBase, new()
        {
            Check.NotNullOrEmpty(alias, "alias");

            var key = new Tuple<Type, string>(typeof(TFields), alias);

            RowFieldsBase cached;
            if (cache.TryGetValue(key, out cached))
                return (TFields)cached;

            var result = new TFields();
            result.Initialize();
            const string error = "$$!!OnlyTableFieldsCanBeUsedAliased!!$$";

            foreach (var field in result)
            {
                if ((field.Flags & FieldFlags.Foreign) == FieldFlags.Foreign |
                    (field.Flags & FieldFlags.ClientSide) == FieldFlags.ClientSide |
                    field.expression == null)
                {
                    field.expression = error;
                }
                else
                {
                    field.expression = JoinAliasLocator.ReplaceAliases(field.expression,
                    x => 
                    {
                        if (x == "t0" || x == "T0")
                            return alias;
                            
                        return error;
                    });
                }

                field.referencedAliases = null;
                field.join = null;
                field.joinAlias = null;
                field.origin = null;
            }
            result.alias = alias;
            result.aliasDot = alias + ".";

            cache[key] = result;
            return result;
        }
    }
}