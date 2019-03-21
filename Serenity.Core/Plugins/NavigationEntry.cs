using System;

namespace Serenity.Plugins
{
    public class NavigationEntry
    {
        public int Order { get; set; }
        public Type Controller { get; set; }
        public string Action { get; set; }
        public string Url { get; set; }
        public string FullPath { get; set; }
        public string Category { get; set; }
        public string Title { get; set; }
        public string IconClass { get; set; }
        public string ItemClass { get; set; }
        public string Permission { get; set; }
        public string Target { get; set; }
    }
}