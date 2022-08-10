namespace Serenity.CodeGeneration
{
    public static class SystemTypes
    {
        public static string ToCSKeyword(string dataType)
        {
            return dataType switch
            {
                "String" => "string",
                "Boolean" => "bool",
                "Byte" => "byte",
                "Char" => "char",
                "Decimal" => "decimal",
                "Double" => "double",
                "Int16" => "short",
                "Int32" => "int",
                "Int64" => "long",
                "Object" => "object",
                "SByte" => "sbyte",
                "Single" => "float",
                "UInt16" => "ushort",
                "UInt32" => "uint",
                "UInt64" => "ulong",
                _ => null
            };
        }

        public static bool IsCSKeyword(string dataType)
        {
            return dataType switch
            {
                "string" => true,
                "bool" => true,
                "byte" => true,
                "char" => true,
                "decimal" => true,
                "double" => true,
                "short" => true,
                "int" => true,
                "long" => true,
                "object" => true,
                "sbyte" => true,
                "float" => true,
                "ushort" => true,
                "uint" => true,
                "ulong" => true,
                _ => false
            };
        }
    }
}