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
        public List<ExternalProperty> Properties { get; set; }
        public List<ExternalMember> Fields { get; set; }
        public List<ExternalMethod> Methods { get; set; }
        public string OptionsType { get; set; }
        public List<ExternalGenericParameter> GenericParameters { get; set; }
        public bool IsAbstract { get; set; }
        public bool IsDeclaration { get; set; }
        public bool IsInterface { get; set; }
        public bool IsSealed { get; set; }
        public bool IsSerializable { get; set; }

        public ExternalType()
        {
            Namespace = "";
            BaseType = "";
            Interfaces = new List<string>();
            Properties = new List<ExternalProperty>();
            Fields = new List<ExternalMember>();
            Methods = new List<ExternalMethod>();
            Attributes = new List<ExternalAttribute>();
            GenericParameters = new List<ExternalGenericParameter>();
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
}