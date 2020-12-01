using Newtonsoft.Json;
using Serenity.Abstractions;
using Serenity.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Serenity.Web
{
    public class LocalTextPackages : Dictionary<string, string[]>
    {
        public const string SectionKey = "LocalTextPackages";

        public LocalTextPackages()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }
    }
}