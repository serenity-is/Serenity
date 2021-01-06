using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace Serenity.Web
{
    public class ScriptBundlingOptions : IOptions<ScriptBundlingOptions>
    {
        public const string SectionKey = "ScriptBundling";

        public ScriptBundlingOptions()
        {
            Bundles = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
            Enabled = false;
            Minimize = true;
            UseMinJS = true;
            Replacements = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public Dictionary<string, string[]> Bundles { get; }
        public bool? Enabled { get; set; }
        public bool? Minimize { get; set; }
        public string[] NoMinimize { get; set; }
        public Dictionary<string, object> Replacements { get; }
        public bool? UseMinJS { get; set; }

        public ScriptBundlingOptions Value => this;
    }
}
