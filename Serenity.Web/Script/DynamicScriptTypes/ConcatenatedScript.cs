using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Serenity.Web
{
    public class ConcatenatedScript : IDynamicScript
    {
        private EventHandler scriptChanged;
        private IEnumerable<Func<string>> scriptParts;

        public ConcatenatedScript(IEnumerable<Func<string>> scriptParts)
        {
            if (scriptParts == null)
                throw new ArgumentNullException("scriptParts");

            this.scriptParts = scriptParts;
        }

        public string GetScript()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var part in scriptParts)
            {
                string partSource = part();

                sb.AppendLine(partSource);
                sb.AppendLine("\r\n");
            }

            return sb.ToString();
        }

        public void CheckRights()
        {
        }

        public void Changed()
        {
            if (scriptChanged != null)
                scriptChanged(this, new EventArgs());
        }

        public bool NonCached
        {
            get { return false; }
        }

        public event EventHandler ScriptChanged 
        {
            add { scriptChanged += value; }
            remove { scriptChanged -= value; }
        }
    }
}