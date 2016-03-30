using System.Collections.Generic;

namespace Serenity.CodeGeneration
{
    public class ExternalType
    {
        public ExternalTypeOrigin Origin { get; set; }
        public string AssemblyName { get; set; }
        public string Namespace { get; set; }
        public string Name { get; set; }
        public string BaseType { get; set; }
        public List<string> Interfaces { get; set; }
        public List<ExternalAttribute> Attributes { get; set; }
        public List<ExternalMember> Members { get; set; }
        public string OptionsType { get; set; }
        public bool HasGenericParameters { get; set; }
        public bool IsAbstract { get; set; }
        public bool IsDeclaration { get; set; }
        public bool IsInterface { get; set; }
        public bool IsSerenity { get; set; }

        public ExternalType()
        {
            Namespace = "";
            BaseType = "";
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
        SS = 2,
        TS = 3
    }
}