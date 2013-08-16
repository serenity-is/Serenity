using System;
using System.Collections.Generic;

namespace Serenity.ComponentModel
{
    public class CustomPropertyInfo
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public string EditorType { get; set; }
        public Dictionary<string, object> EditorParams { get; set; }
        public string Title { get; set; }
        public Func<object, object> GetValue { get; set; }
        public Action<object, object> SetValue { get; set; }
    }
}