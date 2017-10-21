using System;
using System.Collections.Generic;
using System.Text;

namespace Serenity.Web
{
    public class ConcatenatedScript : DynamicScript
    {
        private string separator;
        private IEnumerable<Func<string>> scriptParts;

        public ConcatenatedScript(IEnumerable<Func<string>> scriptParts,
            string separator = "\r\n;\r\n")
        {
            Check.NotNull(scriptParts, "scriptParts");

            this.scriptParts = scriptParts;
            this.separator = separator;
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