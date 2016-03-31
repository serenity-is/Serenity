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
        public bool HasGenericParameters { get; set; }
        public bool IsAbstract { get; set; }
        public bool IsDeclaration { get; set; }
        public bool IsInterface { get; set; }

        public ExternalType()
        {
            Namespace = "";
            BaseType = "";
            Interfaces = new List<string>();
            Properties = new List<ExternalProperty>();
            Fields = new List<ExternalMember>();
            Methods = new List<ExternalMethod>();
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
        public ExternalMember()
        {
            Attributes = new List<ExternalAttribute>();
        }

        public List<ExternalAttribute> Attributes { get; private set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsDeclaration { get; set; }
        public bool IsNullable { get; set; }
        public bool IsProtected { get; set; }
        public bool IsStatic { get; set; }
    }

    public class ExternalMethod : ExternalMember
    {
        public ExternalMethod()
        {
            Arguments = new List<ExternalArgument>();
        }

        public List<ExternalArgument> Arguments { get; private set; }
        public bool IsConstructor { get; set; }
    }

    public class ExternalProperty : ExternalMember
    {
        public ExternalProperty()
        {
        }

        public bool CanGet { get; set; }
        public bool CanSet { get; set; }
    }

    public class ExternalAttribute
    {
        public ExternalAttribute()
        {
            Arguments = new List<ExternalArgument>();
        }

        public string Type { get; set; }
        public List<ExternalArgument> Arguments { get; private set; }
    }

    public class ExternalArgument
    {
        public string Type { get; set; }
        public object Value { get; set; }
        public string Name { get; set; }
        public bool IsOptional { get; set; }
        public bool HasDefault { get; set; }
    }

    public enum ExternalTypeOrigin
    {
        Server = 1,
        SS = 2,
        TS = 3
    }
}