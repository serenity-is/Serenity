using System;
using System.Data;

namespace Serenity.Data
{
    public partial class SqlQuery
    {
        /// <summary>
        ///   SQLSelect'in Select komutları için bir alana karşılık gelen bilgilerin tutulduğu yardımcı 
        ///   sınıf.</summary>
        private class Column
        {
            /// <summary>
            ///   Alanın adı</summary>
            public string Expression;
            /// <summary>
            ///   Varsa alana atanan alias</summary>
            public string AsAlias;
            /// <summary>
            ///   Alan IDataReader'dan hangi Row nesnesinin içine yüklenecek</summary>
            public int IntoRow;
            /// <summary>
            ///   Alan IDataReader'dan hangi Row nesnesinin içine yüklenecek</summary>
            // public Func<Row, Row> IntoSub;
            /// <summary>
            ///   Alan IDataReader'dan hangi Field nesnesinin içine yüklenecek</summary>
            public Field IntoField;

            public Column(string expression, string asAlias, int intoRow, Field intoField)
            {
                this.Expression = expression;
                this.AsAlias = asAlias;
                this.IntoRow = intoRow;
                this.IntoField = intoField;
            }
        }
    }
}