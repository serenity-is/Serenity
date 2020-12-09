using System;
using System.Collections.Generic;

namespace Serenity.Web
{
    public class LocalTextPackages : Dictionary<string, string>
    {
        public const string SectionKey = "LocalTextPackages";

        public LocalTextPackages()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }
    }
}