using Serenity.CodeGeneration;

namespace Serenity.CodeGenerator;

public class ApplicationMetadata : IApplicationMetadata
{
    private class Scanner : TypingsGeneratorBase
    {
        public List<TypeDefinition> RowTypes { get; } = new();

        public Scanner(IGeneratorFileSystem fileSystem, params string[] assemblyLocations)
            : base(fileSystem, assemblyLocations)
        {
        }

        protected override void GenerateCodeFor(TypeDefinition type)
        {
        }

        protected override void ScanAnnotationTypeAttributes(TypeDefinition type)
        {
            base.ScanAnnotationTypeAttributes(type);

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

    private readonly Dictionary<string, IRowMetadata> rowByTablename = new();

    public IRowMetadata GetRowByTablename(string tablename)
    {
        if (tablename is null)
            throw new ArgumentNullException(nameof(tablename));

        if (rowByTablename.TryGetValue(tablename, out IRowMetadata metadata))
            return metadata;

        foreach (var type in scanner.RowTypes)
        {
            var attr = type.GetAttributes().FirstOrDefault(x =>
                x.AttributeType().Name == "TableNameAttribute" &&
                x.AttributeType().NamespaceOf() == "Serenity.Data.Mapping");

            if (attr != null)
            {
                if (IsEqualIgnoreCase(tablename, attr?.ConstructorArguments?.FirstOrDefault().Value as string))
                {
                    rowByTablename[tablename] = metadata = new RowMetadata(type);
                    return metadata;
                }

                continue;
            }

            var name = type.Name;
            if (name.EndsWith("Row", StringComparison.Ordinal))
                name = name[..^3];

            if (IsEqualIgnoreCase(tablename, name))
            {
                rowByTablename[tablename] = metadata = new RowMetadata(type);
                return metadata;
            }
        }

        rowByTablename[tablename] = null;
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

        public bool HasLookupScriptAttribute => type.GetAttributes()
            .Any(x => x.AttributeType?.Name == "LookupScriptAttribute" &&
                x.AttributeType?.NamespaceOf() == "Serenity.ComponentModel");

        public IRowPropertyMetadata GetTableField(string columnName)
        {
            throw new NotImplementedException();
        }
    }
}