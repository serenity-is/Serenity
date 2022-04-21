using Microsoft.Extensions.Options;

namespace Serenity.Web
{
    public class CssBundlingOptions : IOptions<CssBundlingOptions>
    {
        public const string SectionKey = "CssBundling";

        public CssBundlingOptions()
        {
            Bundles = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
            Enabled = false;
            Minimize = true;
            UseMinCSS = true;
            Replacements = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public bool? Enabled { get; set; }
        public bool? Minimize { get; set; }
        public bool? UseMinCSS { get; set; }
        public string[] NoMinimize { get; set; }
        public Dictionary<string, object> Replacements { get; set; }
        public Dictionary<string, string[]> Bundles { get; set; }

        public CssBundlingOptions Value => this;
    }
}
