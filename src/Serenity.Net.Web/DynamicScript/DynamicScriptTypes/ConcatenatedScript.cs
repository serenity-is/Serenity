namespace Serenity.Web
{
    public class ConcatenatedScript : DynamicScript
    {
        private readonly string separator;
        private readonly IEnumerable<Func<DynamicScriptResponseType, string>> scriptParts;
        private readonly Action<IPermissionService, ITextLocalizer> checkRights;

        public ConcatenatedScript(IEnumerable<Func<DynamicScriptResponseType, string>> scriptParts,
            string separator = "\r\n;\r\n", Action<IPermissionService, ITextLocalizer> checkRights = null)
        {
            this.scriptParts = scriptParts ?? throw new ArgumentNullException(nameof(scriptParts));
            this.separator = separator;
            this.checkRights = checkRights;
        }

        public override void CheckRights(IPermissionService permissions, ITextLocalizer localizer)
        {
            base.CheckRights(permissions, localizer);

            checkRights?.Invoke(permissions, localizer);
        }

        public override string GetScript(DynamicScriptResponseType responseType)
        {
            StringBuilder sb = new();

            foreach (var part in scriptParts)
            {
                string partSource = part(responseType);

                sb.AppendLine(partSource);
                if (!string.IsNullOrEmpty(separator))
                    sb.AppendLine(separator);
            }

            return sb.ToString();
        }
    }
}