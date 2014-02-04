namespace Serenity.Data
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Dictionary = System.Collections.Generic.Dictionary<string, object>;

    public class ParameterizedQuery : IDbParameterized
    {
        private Dictionary parameters;
        protected ParameterizedQuery parent;
        private int nextAutoParam;

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

            return new Parameter((nextAutoParam++).IndexParam());
        }
    }
}