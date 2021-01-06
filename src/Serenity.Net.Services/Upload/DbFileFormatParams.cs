using System;

namespace Serenity.Web
{
    public class FormatFilenameOptions
    {
        public string Format { get; set; }
        public Func<string, string> PostFormat { get; set; }
        public object EntityId { get; set; }
        public string OriginalName { get; set; }
    }
}