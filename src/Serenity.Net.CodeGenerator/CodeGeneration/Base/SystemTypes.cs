namespace Serenity.CodeGeneration
{
    public static class SystemTypes
    {
        public static string ToCSKeyword(string dataType)
        {
            return dataType switch
            {
                "System.String" => "string",
                "System.Boolean" => "bool",
                "System.Byte" => "byte",
                "System.Char" => "char",
                "System.Decimal" => "decimal",
                "System.Double" => "double",
                "System.Int16" => "short",
                "System.Int32" => "int",
                "System.Int64" => "long",
                "System.Object" => "object",
                "System.SByte" => "sbyte",
                "System.Single" => "float",
                "System.UInt16" => "ushort",
                "System.UInt32" => "uint",
                "System.UInt64" => "ulong",
                _ => null
            };
        }
    }
}