namespace Serenity.Data
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Dictionary = System.Collections.Generic.Dictionary<string, object>;

    /// <summary>
    ///   Class to generate queries of the form <c>INSERT INTO tablename (field1, field2..fieldN) 
    ///   VALUES (value1, value2..valueN)</c></summary>
    public class SqlInsert : IDbSetFieldTo
    {
        private string _tableName;
        private List<string> _nameValuePairs;
        private Dictionary _params;
        private string _identityColumn;
        private string _cachedQuery;
        private SqlDialect _dialect;

        private void Initialize(string tableName)
        {
            if (tableName == null || tableName.Length == 0)
                throw new ArgumentNullException("tableName");

            _tableName = tableName;
            _nameValuePairs = new List<string>();
            _cachedQuery = null;
            _dialect = SqlSettings.CurrentDialect;
        }

        public SqlDialect Dialect
        {
            get { return _dialect; }
            set { _dialect = value; }
        }

        public string IdentityColumn()
        {
            return _identityColumn;
        }

        public SqlInsert IdentityColumn(string value)
        {
            _identityColumn = value;
            return this;
        }

        /// <summary>
        ///   Creates a new SqlInsert query.</summary>
        /// <param name="tableName">
        ///   Table to insert record (required).</param>
        public SqlInsert(string tableName)
        {
            Initialize(tableName);
        }

        /// <summary>
        ///   Creates a new SqlInsert query.</summary>
        /// <param name="row">
        ///   Row with field values to set in new record (must be in TrackAssignments mode).</param>
        public SqlInsert(Row row)
        {
            if (row == null)
                throw new ArgumentNullException("row");
            Initialize(row.Table);
            this.Set(row);
            if (row is IIdRow)
                this.IdentityColumn(((Field)(((IIdRow)row).IdField)).Name);
        }

        /// <summary>
        ///   Sets field value.</summary>
        /// <param name="field">
        ///   Field name (required).</param>
        /// <param name="value">
        ///   Field value (expression, required).</param>
        /// <returns>
        ///   SqlInsert object itself.</returns>
        public SqlInsert SetTo(string field, string value)
        {
            if (field == null || field.Length == 0)
                throw new ArgumentNullException(field);
            if (value == null || value.Length == 0)
                throw new ArgumentNullException(value);

            _nameValuePairs.Add(field);
            _nameValuePairs.Add(value);
            _cachedQuery = null;
            return this;
        }

        /// <summary>
        ///   Sets field value.</summary>
        /// <param name="field">
        ///   Field name (required).</param>
        /// <param name="value">
        ///   Field value (expression, required).</param>
        /// <returns>
        ///   SqlInsert object itself.</returns>
        void IDbSetFieldTo.SetTo(string field, string value)
        {
            if (field == null || field.Length == 0)
                throw new ArgumentNullException(field);
            if (value == null || value.Length == 0)
                throw new ArgumentNullException(value);

            _nameValuePairs.Add(field);
            _nameValuePairs.Add(value);
            _cachedQuery = null;
        }

        /// <summary>
        ///   Sets field value.</summary>
        /// <param name="meta">
        ///   Field (required).</param>
        /// <param name="value">
        ///   Field value (expression, required).</param>
        /// <returns>
        ///   SqlInsert object itself.</returns>
        public SqlInsert SetTo(Field field, string value)
        {
            if (field == null)
                throw new ArgumentNullException("meta");
            _cachedQuery = null;
            return SetTo(field.Name, value);
        }

        /// <summary>
        ///   Assigns NULL as the field value.</summary>
        /// <param name="field">
        ///   Field (required).</param>
        /// <returns>
        ///   SqlInsert object itself.</returns>
        public SqlInsert SetNull(string field)
        {
            if (String.IsNullOrEmpty(field))
                throw new ArgumentNullException(field);

            _nameValuePairs.Add(field);
            _nameValuePairs.Add(SqlConsts.Null);
            _cachedQuery = null;
            return this;
        }

        /// <summary>
        ///   Sets a parameter value.</summary>
        /// <param name="name">
        ///   Parameter name.</param>
        /// <param name="value">
        ///   Parameter value</param>
        /// <returns>
        ///   SqlInsert object itself.</returns>
        public SqlInsert SetParam(string name, object value)
        {
            if (_params == null)
                _params = new Dictionary();
            _params[name] = value;
            return this;
        }

        ///   Clones the query.</summary>
        /// <returns>
        ///   Clone.</returns>
        public SqlInsert Clone()
        {
            SqlInsert clone = new SqlInsert(_tableName);
            clone._nameValuePairs.AddRange(this._nameValuePairs);
            if (this._params != null)
            {
                clone._params = new Dictionary();
                foreach (var pair in this._params)
                    clone._params.Add(pair.Key, pair.Value);
            }
            clone._cachedQuery = this._cachedQuery;
            return clone;
        }

        /// <summary>
        ///   Gets string representation of the query.</summary>
        /// <returns>
        ///   String representation.</returns>
        public override string ToString()
        {
            if (_cachedQuery != null)
                return _cachedQuery;

            _cachedQuery = Format(_tableName, _nameValuePairs);

            return _cachedQuery;           
        }

        /// <summary>
        ///   Gets params dictionary. May return null if no params set.</summary>
        public Dictionary Params
        {
            get { return _params; }
            set { _params = value; }
        }

        /// <summary>
        ///   Gets param count.</summary>
        public int ParamCount
        {
            get { return _params == null ? 0 : _params.Count; }
        }

        /// <summary>
        ///   Formats an INSERT query.</summary>
        /// <param name="tableName">
        ///   Tablename (required).</param>
        /// <param name="nameValuePairs">
        ///   Field names and values. Must be passed in the order of <c>[field1, value1, field2, 
        ///   value2, ...., fieldN, valueN]</c>. It must have even number of elements.</param>
        /// <returns>
        ///   Formatted query.</returns>
        public static string Format(string tableName, List<string> nameValuePairs)
        {
            if (tableName == null || tableName.Length == 0)
                throw new ArgumentNullException(tableName);

            if (nameValuePairs == null)
                throw new ArgumentNullException("nameValuePairs");

            if (nameValuePairs.Count % 2 != 0)
                throw new ArgumentOutOfRangeException("nameValuePairs");

            StringBuilder sb = new StringBuilder("INSERT INTO ", 64 + nameValuePairs.Count * 16);
            sb.Append(tableName);
            sb.Append(" (");
            for (int i = 0; i < nameValuePairs.Count; i += 2)
            {
                if (i > 0)
                    sb.Append(',');
                sb.Append(nameValuePairs[i]);
            }
            sb.Append(") VALUES (");
            for (int i = 1; i < nameValuePairs.Count; i += 2)
            {
                if (i > 1)
                    sb.Append(',');
                sb.Append(nameValuePairs[i]);
            }
            sb.Append(')');

            return sb.ToString();
        }
    }
}