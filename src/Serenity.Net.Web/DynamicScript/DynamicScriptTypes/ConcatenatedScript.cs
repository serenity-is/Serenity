using Serenity.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Serenity.Web
{
    public class ConcatenatedScript : DynamicScript
    {
        private string separator;
        private IEnumerable<Func<string>> scriptParts;
        private Action<IPermissionService, ITextLocalizer> checkRights;

        public ConcatenatedScript(IEnumerable<Func<string>> scriptParts,
            string separator = "\r\n;\r\n", Action<IPermissionService, ITextLocalizer> checkRights = null)
        {
            this.scriptParts = scriptParts ?? throw new ArgumentNullException(nameof(scriptParts));
            this.separator = separator;
            this.checkRights = checkRights;
        }

        public override void CheckRights(IPermissionService permissions, ITextLocalizer localizer)
        {
            base.CheckRights(permissions, localizer);

            if (checkRights != null)
                checkRights(permissions, localizer);
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