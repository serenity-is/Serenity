
namespace Serenity.Data
{
    public static class SqlConsts
    {
        public const string Null = "NULL";

        private static string[] _indexParam;
        private static string[] _tableAlias;
        private static string[] _tableAliasDot;

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
                for (int i = 0; i < indexParam.Length; i++)
                    indexParam[i] = "@p" + i.ToString();
                _indexParam = indexParam;
            }

            if (param >= 0 && param < _indexParam.Length)
                return _indexParam[param];
            else
                return "@p" + param.ToString();
        }

        public static string TableAlias(this int joinIndex)
        {
            if (_tableAlias == null)
            {
                var tableAlias = new string[100];
                for (int i = 0; i < tableAlias.Length; i++)
                    tableAlias[i] = "T" + i.ToInvariant();
                _tableAlias = tableAlias;
            }

            if (joinIndex >= 0 && joinIndex < _tableAlias.Length)
                return _tableAlias[joinIndex];
            else
                return "T" + joinIndex.ToInvariant();
        }

        public static string TableAliasDot(this int joinIndex)
        {
            if (_tableAliasDot == null)
            {
                var tableAliasDot = new string[100];
                for (int i = 0; i < tableAliasDot.Length; i++)
                    tableAliasDot[i] = "T" + i.ToInvariant() + ".";
                _tableAliasDot = tableAliasDot;
            }

            if (joinIndex >= 0 && joinIndex < _tableAliasDot.Length)
                return _tableAliasDot[joinIndex];
            else
                return "T" + joinIndex.ToInvariant() + ".";
        }
    }
}