using System;
using System.Collections.Generic;
using System.Text;

namespace Serenity.Data
{
    /// <summary>
    /// SQL query elements which is basically used for generating custor query string outside the SqlQuery class. 
    /// eg: inside dialect
    /// </summary>
    public partial class SqlQueryElements
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public Dictionary<string, string> AliasExpressions { get; set; }
        public Dictionary<string, IHaveJoins> AliasWithJoins { get; set; }
        public List<SqlQuery.Column> Columns { get; set; }
        public bool CountRecords { get; set; }
        public bool Distinct { get; set; }
        public StringBuilder From { get; set; }
        public StringBuilder Having { get; set; }
        public StringBuilder GroupBy { get; set; }
        public List<string> OrderBy { get; set; }
        public string ForXml { get; set; }
        public string ForJson { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public StringBuilder Where { get; set; }
        public int IntoIndex { get; set; } = -1;
        public List<object> Into { get; set; } = new List<object>();
        public SqlQuery UnionQuery { get; set; }
        public SqlUnionType UnionType { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

    }
}