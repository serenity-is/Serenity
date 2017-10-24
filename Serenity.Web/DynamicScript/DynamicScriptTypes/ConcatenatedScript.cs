using System;
using System.Collections.Generic;
using System.Text;

namespace Serenity.Web
{
    public class ConcatenatedScript : DynamicScript
    {
        private string separator;
        private IEnumerable<Func<string>> scriptParts;
        private Action checkRights;

        public ConcatenatedScript(IEnumerable<Func<string>> scriptParts,
            string separator = "\r\n;\r\n", Action checkRights = null)
        {
            Check.NotNull(scriptParts, "scriptParts");

            this.scriptParts = scriptParts;
            this.separator = separator;
            this.checkRights = checkRights;
        }

        public override void CheckRights()
        {
            base.CheckRights();

            if (checkRights != null)
                checkRights();
        }

        public override string GetScript()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var part in scriptParts)
            {
                string partSource = part();

                sb.AppendLine(partSource);
                if (!string.IsNullOrEmpty(separator))
                    sb.AppendLine(separator);
            }

            return sb.ToString();
        }
    }
}