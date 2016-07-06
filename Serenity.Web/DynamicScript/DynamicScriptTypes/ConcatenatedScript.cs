using System;
using System.Collections.Generic;
using System.Text;

namespace Serenity.Web
{
    public class ConcatenatedScript : DynamicScript
    {
        private IEnumerable<Func<string>> scriptParts;

        public ConcatenatedScript(IEnumerable<Func<string>> scriptParts)
        {
            Check.NotNull(scriptParts, "scriptParts");

            this.scriptParts = scriptParts;
        }

        public override string GetScript()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var part in scriptParts)
            {
                string partSource = part();

                sb.AppendLine(partSource);
                sb.AppendLine("\r\n\r\n");
            }

            return sb.ToString();
        }
    }
}