using System;
using System.Globalization;

namespace Serenity.Data
{
    public static class SqlSyntax
    {
        private static string[] _indexParam;
        private static string[] _tableAlias;
        private static string[] _tableAliasDot;
        private static CultureInfo _invariant = CultureInfo.InvariantCulture;

        /// <summary>
        ///   Returns an indexed parameter name like @p123.</summary>
        /// <param name="param">
        ///   Param index.</param>
        /// <returns>
        ///   Param name.</returns>
        public static string IndexParam(this int param)
        {
            if (_indexParam == null)
            {
                var indexParam = new string[1000];
                for (int index = 0; index < indexParam.Length; index++)
                    indexParam[index] = "@p" + index;
                _indexParam = indexParam;
            }

            if (param >= 0 && param < _indexParam.Length)
                return _indexParam[param];
            else
                return "@p" + param;
        }

        public static string TableAlias(this int joinIndex)
        {
            if (_tableAlias == null)
            {
                var tableAlias = new string[100];
                for (int i = 0; i < tableAlias.Length; i++)
                    tableAlias[i] = "T" + i.ToString(_invariant);
                _tableAlias = tableAlias;
            }

            if (joinIndex >= 0 && joinIndex < _tableAlias.Length)
                return _tableAlias[joinIndex];
            else
                return "T" + joinIndex.ToString(_invariant);
        }

        public static string TableAliasDot(this int joinIndex)
        {
            if (_tableAliasDot == null)
            {
                var tableAliasDot = new string[100];
                for (int i = 0; i < tableAliasDot.Length; i++)
                    tableAliasDot[i] = "T" + i.ToString(_invariant) + ".";
                _tableAliasDot = tableAliasDot;
            }

            if (joinIndex >= 0 && joinIndex < _tableAliasDot.Length)
                return _tableAliasDot[joinIndex];
            else
                return "T" + joinIndex.ToString(_invariant) + ".";
        }

        public static bool IsValidIdentifier(string s)
        {
            if (string.IsNullOrEmpty(s))
                return false;

            var c = Char.ToUpperInvariant(s[0]);
            if (c != '_' && (c < 'A' || c > 'Z'))
                return false;

            for (var i = 1; i < s.Length; i++)
            {
                c = Char.ToUpperInvariant(s[i]);
                if (c != '_' &&
                    !((c >= '0' && c <= '9') ||
                      (c >= 'A' && c <= 'Z')))
                    return false;
            }

            return true;
        }
    }
}