using Serenity.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Serenity.Web
{
    public class DataScript : DynamicScript, INamedDynamicScript
    {
        private string name;
        private Func<object> getData;

        public DataScript(string name, Func<object> getData)
        {
            Check.NotNull(name, "name");
            Check.NotNull(getData, "getData");

            this.name = name;
            this.getData = getData;
        }

        public string ScriptName { get { return name; } }

        public override string GetScript()
        {
            var data = getData();
            return String.Format("Q$ScriptData.set({0}, {1});", name.ToSingleQuoted(), data.ToJson());
        }
    }
}