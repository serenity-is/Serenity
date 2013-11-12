using System;
using System.Text;

namespace Serenity.Data
{
    public static partial class Sql
    {
        public static partial class Keyword
        {
            public const string Select = "SELECT ";
            public const string Distinct = "DISTINCT ";
            public const string From = " \nFROM ";
            public const string Where = " \nWHERE ";
            public const string OrderBy = " \nORDER BY ";
            public const string GroupBy = " \nGROUP BY ";
            public const string Having = " \nHAVING ";
            public const string And = " AND ";
            public const string Desc = " DESC";
            public const string As = " AS ";
        }
    }
}