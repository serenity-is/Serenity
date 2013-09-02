namespace Serenity.Data
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Dictionary = System.Collections.Generic.Dictionary<string, object>;

    public class ParameterizedQuery : IDbParameterized
    {
        private Dictionary parameters;
        private int nextAutoParam;

        public void AddParam(string name, object value)
        {
            if (parameters == null)
                parameters = new Dictionary();

            parameters.Add(name, value);
        }

        public void SetParam(string name, object value)
        {
            if (parameters == null)
                parameters = new Dictionary();

            parameters[name] = value;
        }

        public Dictionary<string, object> Params
        {
            get { return parameters; }
            set { parameters = value; }
        }

        public int NextAutoParam
        {
            get { return nextAutoParam; }
            set { nextAutoParam = value; }
        }

        public int ParamCount
        {
            get { return parameters == null ? 0 : parameters.Count; }
        }

        public Parameter AutoParam()
        {
            return new Parameter((nextAutoParam++).IndexParam());
        }
    }
}