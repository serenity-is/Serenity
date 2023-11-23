using Serenity.CodeGeneration;

namespace Serenity.CodeGenerator;

public class ApplicationMetadata : IApplicationMetadata
{
    private class Scanner(IFileSystem fileSystem, params string[] assemblyLocations) : TypingsGeneratorBase(fileSystem, assemblyLocations)
    {
        public List<TypeDefinition> RowTypes { get; } = [];
        public Dictionary<string, string> RowTypeToListRoute = [];

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
            else if (TypingsUtils.IsSubclassOf(type, "Serenity.Services", "ServiceEndpoint"))
            {
                var route = type.GetAttributes().FirstOrDefault(x => x.AttributeType?.Name == "RouteAttribute" &&
                    x.AttributeType?.Namespace == "Microsoft.AspNetCore.Mvc")?.ConstructorArguments?.FirstOrDefault().Value as string;
                if (!string.IsNullOrEmpty(route) &&
                    route.EndsWith("/[action]", StringComparison.Ordinal))
                {
                    route = route[..^("/[action]".Length)];
                    if (route.StartsWith("~/", StringComparison.Ordinal))
                        route = route[2..];
                    else if (route.StartsWith('/'))
                        route = route[1..];

                    if (!string.IsNullOrEmpty(route))
                    {
                        var method = type.MethodsOf().FirstOrDefault(x => x.Name == "List" &&
                            x.ReturnType?.Name == "ListResponse`1" &&
                            x.ReturnType.Namespace == "Serenity.Services");

                        if (method != null &&
                            method.ReturnType is GenericInstanceType git &&
                            git?.GenericArguments()?.Count > 0)
                        {
                            var rowType = git.GenericArguments[0]?.FullNameOf();
                            if (rowType != null)
                                RowTypeToListRoute[rowType] = route + "/List";
                        }
                    }
                }
            }
        }

        protected override void HandleMemberType(TypeReference memberType, string codeNamespace, bool module)
        {
        }
    }

    private readonly Scanner scanner;

    public ApplicationMetadata(IFileSystem fileSystem, params string[] assemblyLocations)
    {
        scanner = new Scanner(fileSystem, assemblyLocations);
        scanner.Run();
        foreach (var asm in scanner.Assemblies)
            asm.Dispose();
    }

    public List<EntityModel> EntityModels { get; } = [];
    public string DefaultSchema { get; set; }

    private string ParseSchemaAndName(string objectName, out string schema)
    {
        ArgumentNullException.ThrowIfNull(objectName);

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

    private readonly Dictionary<string, IRowMetadata> rowByTablename = [];

    public IRowMetadata GetRowByTablename(string tablename)
    {
        ArgumentNullException.ThrowIfNull(tablename);

        if (rowByTablename.TryGetValue(tablename, out IRowMetadata metadata))
            return metadata;

        foreach (var model in EntityModels)
        {
            if (IsEqualIgnoreCase(tablename, model.SchemaAndTable))
                return rowByTablename[tablename] = metadata = new EntityModelRowMetadata(model);
        }

        foreach (var type in scanner.RowTypes)
        {
            var attr = type.GetAttributes().FirstOrDefault(x =>
                x.AttributeType().Name == "TableNameAttribute" &&
                x.AttributeType().NamespaceOf() == "Serenity.Data.Mapping");

            RowMetadata createRowMetadata()
            {
                var metadata = new RowMetadata(type);
                if (scanner.RowTypeToListRoute.TryGetValue(type.FullNameOf(), out string route))
                    metadata.ListServiceRoute = route;
                rowByTablename[tablename] = metadata;
                return metadata;
            }

            if (attr != null)
            {
                if (IsEqualIgnoreCase(tablename, attr?.ConstructorArguments?.FirstOrDefault().Value as string))
                    return createRowMetadata();

                continue;
            }

            var name = type.Name;
            if (name.EndsWith("Row", StringComparison.Ordinal))
                name = name[..^3];

            if (IsEqualIgnoreCase(tablename, name))
                return createRowMetadata();
        }

        rowByTablename[tablename] = null;
        return null;
    }

    private static string GetColumnName(PropertyDefinition x)
    {
        var attrs = x.GetAttributes();
        var columnAttr = attrs?.FirstOrDefault(z => z.AttributeType?.Name == "ColumnAttribute" &&
            z.AttributeType.NamespaceOf() == "Serenity.Data.Mapping");

        if (columnAttr != null)
        {
            if (columnAttr.ConstructorArguments?.FirstOrDefault().Value is string c)
                return SqlSyntax.Unquote(c);
        }

        if (attrs.Any(x => x.AttributeType?.Name == "OriginAttribute" &&
            x.AttributeType?.NamespaceOf() == "Serenity.Data.Mapping"))
            return null;

        if (attrs.Any(x => x.AttributeType != null &&
            TypingsUtils.IsSubclassOf(x.AttributeType, "Serenity.Data.Mapping",
            "BaseExpressionAttribute")))
            return null;

        return x.Name;
    }

    private class RowMetadata(TypeDefinition type) : IRowMetadata
    {
        private readonly TypeDefinition type = type ?? throw new ArgumentNullException(nameof(type));
        private string idProperty;
        private string nameProperty;

        public string Namespace => type.NamespaceOf();

        public string ClassName => type.Name;

        public string FullName => type.FullNameOf();

        public string Module => type.GetAttributes()
            .FirstOrDefault(x => x.AttributeType?.Name == "ModuleAttribute" &&
                x.AttributeType?.NamespaceOf() == "Serenity.ComponentModel")?.ConstructorArguments?.FirstOrDefault().Value as string;

        public bool HasLookupScriptAttribute => type.GetAttributes()
            .Any(x => x.AttributeType?.Name == "LookupScriptAttribute" &&
                x.AttributeType?.NamespaceOf() == "Serenity.ComponentModel");

        private readonly Dictionary<string, IRowPropertyMetadata> tableFieldByColumnName = [];

        public IRowPropertyMetadata GetTableField(string columnName)
        {
            if (string.IsNullOrEmpty(columnName))
                return null;

            columnName = SqlSyntax.Unquote(columnName);

            if (tableFieldByColumnName.TryGetValue(columnName, out IRowPropertyMetadata metadata))
                return metadata;

            var props = type.PropertiesOf().Where(x => string.Equals(GetColumnName(x), columnName,
                StringComparison.OrdinalIgnoreCase));

            if (props.Count() == 1)
                return tableFieldByColumnName[columnName] = new PropertyMetadata(props.First());

            return tableFieldByColumnName[columnName] = null;
        }

        private readonly Dictionary<string, IRowPropertyMetadata> propertyByName = [];

        public IRowPropertyMetadata GetProperty(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            if (propertyByName.TryGetValue(name, out IRowPropertyMetadata metadata))
                return metadata;

            var prop = type.PropertiesOf().FirstOrDefault(x => x.Name == name);
            if (prop != null)
                return propertyByName[name] = new PropertyMetadata(prop);

            return propertyByName[name] = null;
        }

        public string IdProperty
        {
            get
            {
                if (idProperty == null)
                {
                    var props = type.PropertiesOf().Where(p => p.GetAttributes(
                        "Serenity.Data", "IdPropertyAttribute", subAttributes: false).Any());

                    idProperty = props.Count() == 1 ? props.First().Name : string.Empty;
                }

                return idProperty == string.Empty ? null : idProperty;
            }
        }

        public string NameProperty
        {
            get
            {
                if (nameProperty == null)
                {
                    var props = type.PropertiesOf().Where(p => p.GetAttributes(
                        "Serenity.Data", "NamePropertyAttribute", subAttributes: false).Any());

                    nameProperty = props.Count() == 1 ? props.First().Name : string.Empty;
                }

                return nameProperty == string.Empty ? null : nameProperty;
            }
        }

        public class PropertyMetadata(PropertyDefinition property) : IRowPropertyMetadata
        {
            private readonly PropertyDefinition property = property ?? throw new ArgumentNullException(nameof(property));

            public string ColumnName => GetColumnName(property);
            public string PropertyName => property.Name;
        }

        public string ListServiceRoute { get; set; }
    }

    private class EntityModelRowMetadata(EntityModel model) : IRowMetadata
    {
        private readonly EntityModel model = model ?? throw new ArgumentNullException(nameof(model));

        public bool HasLookupScriptAttribute => false;

        public string ListServiceRoute => "Services/" + model.ServiceBaseUrl;

        public string IdProperty => model.IdField;

        public string NameProperty => model.NameField;

        public string Module => model.Module;

        public string Namespace => model.ModuleNamespace;

        public string ClassName => model.RowClassName;

        private readonly Dictionary<string, IRowPropertyMetadata> propertyByName = [];

        public IRowPropertyMetadata GetProperty(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            if (propertyByName.TryGetValue(name, out IRowPropertyMetadata metadata))
                return metadata;

            var prop = model.Fields.FirstOrDefault(x => x.PropertyName == name);
            if (prop != null)
                return propertyByName[name] = new FieldMetadata(prop);

            return propertyByName[name] = null;
        }

        private readonly Dictionary<string, IRowPropertyMetadata> tableFieldByColumnName = [];

        public IRowPropertyMetadata GetTableField(string columnName)
        {
            if (string.IsNullOrEmpty(columnName))
                return null;

            columnName = SqlSyntax.Unquote(columnName);

            if (tableFieldByColumnName.TryGetValue(columnName, out IRowPropertyMetadata metadata))
                return metadata;

            var props = model.Fields.Where(x => string.Equals(x.Name, columnName,
                StringComparison.OrdinalIgnoreCase));

            if (props.Count() == 1)
                return tableFieldByColumnName[columnName] = new FieldMetadata(props.First());

            return tableFieldByColumnName[columnName] = null;
        }

        public class FieldMetadata(EntityField field) : IRowPropertyMetadata
        {
            private readonly EntityField field = field ?? throw new ArgumentNullException(nameof(field));

            public string ColumnName => field.Name;
            public string PropertyName => field.PropertyName;
        }
    }
}