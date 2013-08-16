namespace Serenity.Data
{
    using System;
    using System.Text;
    using Dictionary = System.Collections.Generic.Dictionary<string, object>;

    /// <summary>
    ///   Class to generate queries of form <c>DELETE FROM tablename WHERE [conditions]</c>.</summary>
    public sealed class SqlDelete : IDbFilterable
    {
        private string _tableName;
        private StringBuilder _where;
        private Dictionary _params;
        private int _autoParam;

        private void Initialize(string tableName)
        {
            if (tableName == null)
                throw new ArgumentNullException("tableName");

            _tableName = tableName;
            _where = new StringBuilder();
        }

        /// <summary>
        ///   Creates a new SqlDelete query.</summary>
        /// <param name="tableName">
        ///   Table to delete records from (required).</param>
        public SqlDelete(string tableName)
        {
            Initialize(tableName);
        }

        /// <summary>
        ///   Creates a new SqlDelete query.</summary>
        /// <param name="row">
        ///   Row with values to set in key record (must be in TrackAssignments mode).</param>
        public SqlDelete(Row row)
        {
            if (row == null)
                throw new ArgumentNullException("row");
            Initialize(row.Table);
            this.WhereEqual(row);
        }

        /// <summary>
        ///   Adds a new condition to the WHERE part of the query with an "AND" between.</summary>
        /// <param name="condition">
        ///   Condition.</param>
        /// <returns>
        ///   SqlDelete object itself.</returns>
        public SqlDelete Where(string condition)
        {
            if (condition == null || condition.Length == 0)
                throw new ArgumentNullException("condition");

            condition = SqlUpdate.RemoveT0Reference(condition);

            if (_where.Length > 0)
                _where.Append(Sql.Keyword.And);

            _where.Append(condition);

            return this;
        }

        /// <summary>
        ///   Adds a new condition to the WHERE part of the query with an "AND" between.</summary>
        /// <param name="condition">
        ///   Condition.</param>
        /// <returns>
        ///   SqlDelete object itself.</returns>
        void IDbFilterable.Where(string condition)
        {
            Where(condition);
        }

        /// <summary>
        ///   Adds new conditions to the WHERE part of the query with an "AND" between.</summary>
        /// <param name="conditions">
        ///   Conditions.</param>
        /// <returns>
        ///   SqlDelete object itself.</returns>
        public SqlDelete Where(params string[] conditions)
        {
            if (conditions == null || conditions.Length == 0)
                throw new ArgumentNullException("conditions");

            foreach (var condition in conditions)
                Where(condition);

            return this;
        }

        /// <summary>
        ///   Gets string representation of the query.</summary>
        /// <returns>
        ///   String representation of the query.</returns>
        public override string ToString()
        {
            return Format(_tableName, _where.ToString());
        }

        /// <summary>
        ///   Sets a parameter value.</summary>
        /// <param name="name">
        ///   Parameter name.</param>
        /// <param name="value">
        ///   Parameter value</param>
        /// <returns>
        ///   SqlSelect object itself.</returns>
        public Parameter AutoParam()
        {
            return new Parameter((++_autoParam).IndexParam());
        }

        /// <summary>
        ///   Sets a parameter value.</summary>
        /// <param name="name">
        ///   Parameter name.</param>
        /// <param name="value">
        ///   Parameter value</param>
        /// <returns>
        ///   SqlDelete object itself.</returns>
        public SqlDelete SetParam(string name, object value)
        {
            if (_params == null)
                _params = new Dictionary();
            _params[name] = value;
            return this;
        }

        /// <summary>
        ///   Sets a parameter value.</summary>
        /// <param name="name">
        ///   Parameter name.</param>
        /// <param name="value">
        ///   Parameter value</param>
        /// <returns>
        ///   SqlDelete object itself.</returns>
        void IDbParameterized.SetParam(string name, object value)
        {
            if (_params == null)
                _params = new Dictionary();
            _params[name] = value;
        }

        /// <summary>
        ///   Gets params dictionary. May return null if no params set.</summary>
        public Dictionary Params
        {
            get { return _params; }
        }

        /// <summary>
        ///   Gets param count.</summary>
        public int ParamCount 
        { 
            get { return _params == null ? 0 : _params.Count; }
        }

        /// <summary>
        ///   Formats a DELETE query.</summary>
        /// <param name="tableName">
        ///   Tablename.</param>
        /// <param name="where">
        ///   Where part of the query.</param>
        /// <returns>
        ///   Formatted query.</returns>
        public static string Format(string tableName, string where)
        {
            if (tableName == null || tableName.Length == 0)
                throw new ArgumentNullException(tableName);

            StringBuilder sb = new StringBuilder("DELETE FROM ", 24 + where.Length);
            sb.Append(tableName);

            if (!String.IsNullOrEmpty(where))
            {
                sb.Append(" WHERE ");
                sb.Append(where);
            }
            return sb.ToString();
        }
    }
}