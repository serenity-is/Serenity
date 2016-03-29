using System.Collections.Generic;

namespace Serenity.CodeGeneration
{
    public class ScriptTypeInfo
    {
        public string FullName { get; set; }
        public List<string> BaseClasses { get; set; }
        public List<string> Interfaces { get; set; }
        public bool IsAbstract { get; set; }
        public bool IsDeclaration { get; set; }
        public bool IsGeneric { get; set; }
        public bool IsWidget { get; set; }
        public bool IsEditor { get; set; }
        public bool IsFormatter { get; set; }
        public Dictionary<string, ScriptMemberInfo> Members { get; set; }
        public ScriptTypeInfo OptionsType { get; set; }
    }

    public class ScriptMemberInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsDeclaration { get; set; }
        public bool IsOption { get; set; }
    }
}