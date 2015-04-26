namespace Serenity.Data
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Dictionary = System.Collections.Generic.Dictionary<string, object>;

    [DebuggerDisplay("{DebugText}")]
    public class QueryWithParams : IQueryWithParams
    {
        protected SqlDialect dialect;
        protected QueryWithParams parent;
        private Dictionary parameters;
        private int nextAutoParam;

        public QueryWithParams()
        {
            dialect = SqlSettings.CurrentDialect;
        }

        protected void CloneParams(QueryWithParams target)
        {
            if (this.parameters != null)
            {
                var p = new Dictionary();
                foreach (var pair in this.parameters)
                    p.Add(pair.Key, pair.Value);

                target.parameters = p;
            }

            target.parameters = null;
        }

        public void AddParam(string name, object value)
        {
            if (parent != null)
            {
                parent.AddParam(name, value);
                return;
            }

            if (parameters == null)
                parameters = new Dictionary();

            parameters.Add(name, value);
        }

        public void SetParam(string name, object value)
        {
            if (parent != null)
            {
                parent.SetParam(name, value);
                return;
            }

            if (parameters == null)
                parameters = new Dictionary();

            parameters[name] = value;
        }

        public IDictionary<string, object> Params
        {
            get 
            {
                if (parent != null)
                    return parent.Params;

                return parameters; 
            }
        }

        public int ParamCount
        {
            get 
            {
                if (parent != null)
                    return parent.ParamCount;

                return parameters == null ? 0 : parameters.Count; 
            }
        }

        public Parameter AutoParam()
        {
            if (parent != null)
                return parent.AutoParam();

            return new Parameter((++nextAutoParam).IndexParam());
        }

        /// <summary>
        /// Creates a new query that shares parameter dictionary with this query.
        /// </summary>
        /// <returns>
        /// A new query that shares parameters.</returns>
        public TQuery CreateSubQuery<TQuery>()
            where TQuery: QueryWithParams, new()
        {
            var subQuery = new TQuery
            {
                parent = this
            };
            return subQuery;
        }

        SqlDialect IQueryWithParams.Dialect
        {
            get { return dialect; }
        }

        public string DebugText
        {
            get
            {
                return SqlDebugDumper.Dump(ToString(), this.Params);
            }
        }
    }
}