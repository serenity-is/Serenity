using System;
using Serenity.Data;
using System.Collections.Generic;

namespace Serenity.Data
{
    /// <summary>
    ///   Helper class with some static functions operating on Filter Panel types.</summary>
    public static class FilterFieldExtensions
    {
        /// <summary>
        ///   Filter operator keys used in javascript</summary>
        private static string[] filterOpKeys = 
        {
            "true",
            "false",
            "contains",
            "startswith",
            "eq",
            "ne",
            "gt",
            "ge",
            "lt",
            "le",
            "bw",
            "in", 
            "isnull",
            "isnotnull"
        };

        /// <summary>
        ///   Converts a FilterFieldType enum to its string key used in javascript</summary>
        /// <param name="type">
        ///   A filter field type</param>
        /// <returns>
        ///   The string key corresponding to the field type</returns>
        //internal static string ToStringKey(this FilterFieldType type)
        //{
        //    return fieldTypeKeys[(int)type];
        //}

        /// <summary>
        ///   Converts a FilterOp enum to its string key used in javascript</summary>
        /// <param name="op">
        ///   A filter operator</param>
        /// <returns>
        ///   The string key corresponding to the filter operator</returns>
        internal static string ToStringKey(this FilterOp op)
        {
            return filterOpKeys[(int)op];
        }

        /// <summary>
        ///   Checks if the operator requires a value.</summary>
        /// <param name="op">
        ///   Operator</param>
        /// <returns>
        ///   True if the operator requires a value.</returns>
        /// <remarks>
        ///   True, False, IsNull and IsNotNull operators doesn't require a value.</remarks>
        internal static bool IsNeedsValue(this FilterOp op)
        {
            return op != FilterOp.True && op != FilterOp.False && op != FilterOp.IsNull && op != FilterOp.IsNotNull;
        }

        /// <summary>
        ///   Checks if the operator is one of Contains and StartsWith.</summary>
        /// <param name="op">
        ///   Operator</param>
        /// <returns>
        ///   True if operator is Contains or StartsWith.</returns>
        internal static bool IsLike(this FilterOp op)
        {
            return op == FilterOp.StartsWith || op == FilterOp.Contains;
        }

        /// <summary>
        ///   Tries to find the filter operator corresponding to a operator key used in javascript.</summary>
        /// <param name="filterOp">
        ///   Operator key</param>
        /// <param name="op">
        ///   Operator is returned through this parameter.</param>
        /// <returns>
        ///   True if "filterOp" is one of valid operator keys.</returns>
        internal static bool TryParse(string filterOp, out FilterOp op)
        {
            for (int i = 0; i < filterOpKeys.Length; i++)
                if (filterOpKeys[i] == filterOp)
                {
                    op = (FilterOp)(i);
                    return true;
                }

            op = FilterOp.EQ;           
            return false;
        }

        public static FilterField Boolean(this FilterField field)
        {
            field.Handler("Boolean").Required(true);
            return field;
        }

        public static FilterField Lookup(this FilterField field, string lookupKey)
        {
            field.Handler("Lookup").Param("LookupKey", lookupKey);
            return field;
        }

        public static FilterField Enum(this FilterField field, Type enumType, string enumKey = null)
        {
            field.Handler("Enum");
            field.Param("Values", GetEnumOptionList(enumType, enumKey ?? enumType.Name));
            return field;
        }

        public static List<string[]> GetEnumOptionList(Type enumType)
        {
            if (enumType == null)
                throw new ArgumentNullException("enumType");

            return GetEnumOptionList(enumType, enumType.Name);
        }

        public static List<string[]> GetEnumOptionList(Type enumType, string enumKey)
        {
            if (enumType == null)
                throw new ArgumentNullException("enumType");

            if (!enumType.IsEnum &&
                !enumType.IsSubclassOf(typeof(DataEnum)))
                throw new ArgumentOutOfRangeException("enumType");

            var list = new List<string[]>();

            if (enumType.IsEnum)
            {
                foreach (var p in System.Enum.GetValues(enumType))
                {
                    string key = System.Enum.GetName(enumType, p);
                    string textKey = "Enums." + enumKey + "." + key;
                    string text = LocalText.TryGet(textKey) ?? textKey;
                    list.Add(new string[] { Convert.ToInt32(p).ToInvariant(), text });
                }
            }
            else
            {
                foreach (var p in DataEnumCache.EnumType(enumKey).InDisplayOrder)
                {
                    string key = p.ValueKey;
                    string textKey = "Enums." + enumKey + "." + key;
                    string text = LocalText.TryGet(textKey) ?? textKey;
                    list.Add(new string[] { p.ValueId.ToInvariant(), text });
                }
            }

            return list;
        }

        /*/// <summary>
        ///   Populates values for a filter field from a IDbLookupRow's cache.</summary>
        /// <param name="field">
        ///   Filter field object (required).</param>
        /// <param name="row">
        ///   IDbLookupRow (required).</param>
        /// <returns></returns>
        public static FilterField List(this FilterField field, IDbLookupRow row)
        {
            if (field == null)
                throw new ArgumentNullException("field");
            if (row == null)
                throw new ArgumentNullException("row");

            field.Type(FilterFieldType.List);
            return field;
        }

        /// <summary>
        ///   Populates values for a filter field from a IDbLookupRow's cache.</summary>
        /// <param name="field">
        ///   Filter field object (required).</param>
        /// <param name="row">
        ///   IDbLookupRow (required).</param>
        /// <returns></returns>
        public static FilterField Lookup(this FilterField field, IDbLookupRow row)
        {
            if (field == null)
                throw new ArgumentNullException("field");
            if (row == null)
                throw new ArgumentNullException("row");

            field.Type(FilterFieldType.List);
            return field;
        }*/
    }
}