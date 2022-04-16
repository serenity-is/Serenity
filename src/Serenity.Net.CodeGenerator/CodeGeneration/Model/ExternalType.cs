using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Serenity.CodeGeneration
{
    public class ExternalType
    {
        public string AssemblyName { get; set; }
        public string Namespace { get; set; }
        public string Name { get; set; }
        public string BaseType { get; set; }
        public List<string> Interfaces { get; set; }
        [JsonInclude]
        public List<ExternalAttribute> Attributes { get; set; }
        [JsonInclude]
        public List<ExternalProperty> Properties { get; set; }
        [JsonInclude]
        public List<ExternalMember> Fields { get; set; }
        [JsonInclude]
        public List<ExternalMethod> Methods { get; set; }
        public string OptionsType { get; set; }
        [JsonInclude]
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