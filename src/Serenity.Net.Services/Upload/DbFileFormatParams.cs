using System;

namespace Serenity.Web
{
    public class FormatDbFilenameOptions
    {
        public string DbFileFormat { get; set; }
        public Func<string, string> DbFileReplacer { get; set; }
        public object EntityId { get; set; }
        public string OriginalName { get; set; }
    }
}