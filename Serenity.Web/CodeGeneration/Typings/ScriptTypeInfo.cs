using System.Collections.Generic;

namespace Serenity.CodeGeneration
{
    public class ExternalType
    {
        public ExternalTypeOrigin Origin { get; set; }
        public string Namespace { get; set; }
        public string Name { get; set; }
        public List<string> BaseClasses { get; set; }
        public List<string> Interfaces { get; set; }
        public List<ExternalAttribute> Attributes { get; set; }
        public List<ExternalMember> Members { get; set; }
        public ExternalType OptionsType { get; set; }
        public bool IsAbstract { get; set; }
        public bool IsDeclaration { get; set; }
        public bool IsDialog { get; set; }
        public bool IsEditor { get; set; }
        public bool IsEnum { get; set; }
        public bool IsFormatter { get; set; }
        public bool IsGeneric { get; set; }
        public bool IsGrid { get; set; }
        public bool IsInterface { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsSerenity { get; set; }
        public bool IsStatic { get; set; }
        public bool IsWidget { get; set; }

        public ExternalType()
        {
            Namespace = "";
            BaseClasses = new List<string>();
            Interfaces = new List<string>();
            Members = new List<ExternalMember>();
            Attributes = new List<ExternalAttribute>();
        }

        public string FullName
        {
            get
            {
                return Namespace.IsEmptyOrNull() ? Name :
                    Namespace + "." + Name;
            }
        }

    }

    public class ExternalMember
    {
        public List<ExternalAttribute> Attributes { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsDeclaration { get; set; }
        public bool IsNullable { get; set; }
        public bool IsOption { get; set; }
        public bool IsPrivate { get; set; }
    }

    public class ExternalAttribute
    {
        public string AttributeType { get; set; }
        public Dictionary<string, object> Arguments { get; set; }
    }

    public enum ExternalTypeOrigin
    {
        Server = 1,
        SharpScript = 2,
        TypeScript = 3
    }
}