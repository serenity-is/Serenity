namespace Serenity.Data
{
    using System.Collections.Generic;
    using Dictionary = System.Collections.Generic.Dictionary<string, object>;

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

        SqlDialect IQueryWithParams.Dialect
        {
            get { return dialect; }
        }
    }
}