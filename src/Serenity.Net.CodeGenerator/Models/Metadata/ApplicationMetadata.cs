using Serenity.CodeGeneration;

namespace Serenity.CodeGenerator;

public class ApplicationMetadata : IApplicationMetadata
{
    private class Scanner : ServerTypingsGenerator
    {
        public List<TypeDefinition> RowTypes { get; } = new();

        public Scanner(IGeneratorFileSystem fileSystem, params string[] assemblyLocations)
            : base(fileSystem, assemblyLocations)
        {
        }

        protected override void GenerateCodeFor(TypeDefinition type)
        {
            if (TypingsUtils.IsSubclassOf(type, "Serenity.Data", "Row") ||
                TypingsUtils.IsSubclassOf(type, "Serenity.Data", "Row`1"))
            {
                RowTypes.Add(type);
            }
        }

        protected override void HandleMemberType(TypeReference memberType, string codeNamespace, bool module)
        {
        }
    }

    private readonly Scanner scanner;

    public ApplicationMetadata(IGeneratorFileSystem fileSystem, params string[] assemblyLocations)
    {
        scanner = new Scanner(fileSystem, assemblyLocations);
        scanner.Run();
    }

    string DefaultSchema { get; set; }

    private string ParseSchemaAndName(string objectName, out string schema)
    {
        if (objectName is null)
            throw new ArgumentNullException(nameof(objectName));

        schema = DefaultSchema;
        var parts = objectName.Split('.');
        if (parts.Length <= 0 || parts.Length > 2)
            return null;

        if (parts.Length == 1)
            return SqlSyntax.Unquote(parts[0]);

        schema = SqlSyntax.Unquote(parts[0]);
        return SqlSyntax.Unquote(parts[1]);
    }

    private string NormalizeTablename(string objectName)
    {
        var table = ParseSchemaAndName(objectName, out var schema);
        return table + "." + schema;
    }

    private bool IsEqualIgnoreCase(string objectName1, string objectName2)
    {
        return !string.IsNullOrEmpty(objectName1) &&
            !string.IsNullOrEmpty(objectName2) &&
            string.Equals(NormalizeTablename(objectName1), 
                NormalizeTablename(objectName2), StringComparison.OrdinalIgnoreCase);
    }

    public IRowMetadata GetRowByTablename(string tablename)
    {
        foreach (var type in scanner.RowTypes)
        {
            var attr = type.GetAttributes().FirstOrDefault(x =>
                x.AttributeType().Name == "TableNameAttribute" &&
                x.AttributeType().NamespaceOf() == "Serenity.Data.Mapping");

            if (attr != null)
            {
                if (IsEqualIgnoreCase(tablename, attr?.ConstructorArguments?.FirstOrDefault().Value as string))
                    return new RowMetadata(type);

                continue;
            }

            var name = type.Name;
            if (name.EndsWith("Row", StringComparison.Ordinal))
                name = name[..^3];

            if (IsEqualIgnoreCase(tablename, name))
                return new RowMetadata(type);
        }

        return null;
    }

    private class RowMetadata : IRowMetadata
    {
        private readonly TypeDefinition type;

        public RowMetadata(TypeDefinition type)
        {
            this.type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public string Namespace => type.NamespaceOf();

        public string ClassName => type.Name;
        
        public string FullName => type.FullNameOf();

        public IRowPropertyMetadata GetTableField(string columnName)
        {
            throw new NotImplementedException();
        }
    }
}