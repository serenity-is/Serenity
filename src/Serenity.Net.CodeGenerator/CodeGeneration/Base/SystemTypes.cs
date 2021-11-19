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
    }
}