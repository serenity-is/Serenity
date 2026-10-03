namespace Serenity.CodeGenerator;

public class EntityModel
{
    public string? Module { get; set; }
    public string ConnectionKey { get; set; } = null!;
    public string? Permission { get; set; } = null!;
    public string? RootNamespace { get; set; }
    public string ClassName { get; set; } = null!;
    public string RowClassName { get; set; } = null!;
    public string? Schema { get; set; }
    public string Tablename { get; set; } = null!;
    public string? Title { get; set; }
    public string? IdField { get; set; }
    public string RowBaseClass { get; set; } = "Serenity.Data.Row";
    public List<EntityField> RowBaseFields { get; } = [];
    public string FieldsBaseClass { get; set; } = "Serenity.Data.RowFieldsBase";
    public string? ServiceLookupPermission { get; set; }
    public List<EntityField> Fields { get; } = [];
    public List<EntityJoin> Joins { get; } = [];
    public string? NameField { get; set; }
    public string FieldPrefix { get; set; } = null!;
    public bool AspNetCore { get; set; } = true;
    public bool NET5Plus { get; set; } = true;
    public bool NET8Plus { get; set; } = true;
    public bool NullableRefTypes { get; set; } = false;
    public bool DeclareJoinConstants { get; set; }
    public bool EnableGenerateFields { get; set; }
    public bool EnableGenerateInterface { get; set; }
    public bool EnableRowTemplates { get; set; }
    public bool FileScopedNamespaces { get; set; }
    public bool GenerateListExcel { get; set; }
    public HashSet<string> GlobalUsings { get; } = [];

    public string? Identity => IdField;
    public Dictionary<string, object>? CustomSettings { get; set; }

    public IEnumerable<EntityField> FormFields => Fields.Where(f => !f.OmitInForm);
    public IEnumerable<EntityField> GridFields => Fields.Where(f => !f.OmitInGrid);

    /// <summary>
    /// Gets the module dot prefix, which is a dot followed by the module name.
    /// Can be used inside namespaces or local text keys.
    /// If the module is null or empty, returns an empty string.
    /// </summary>
    public string DotModule
    {
        get { return string.IsNullOrEmpty(Module) ? "" : "." + Module; }
    }

    /// <summary>
    /// Gets the module dot, which is the module name with a trailing dot.
    /// Can be used inside namespaces or form/column keys.
    /// If the module is null or empty, returns an empty string.
    /// </summary>
    public string ModuleDot
    {
        get { return string.IsNullOrEmpty(Module) ? "" : Module + "."; }
    }

    /// <summary>
    /// Gets the module navigation prefix, which uses path separators for dots and has a trailing slash.
    /// If the module is null or empty, returns an empty string.
    /// </summary>
    public string ModuleNavigationPrefix
    {
        get { return ModulePathPrefix; }
    }

    /// <summary>
    /// Gets the module path prefix, which is the module name with dots replaced by slashes and a trailing slash.
    /// If the module is null or empty, returns an empty string.
    /// </summary>
    public string ModulePathPrefix
    {
        get { return string.IsNullOrEmpty(Module) ? "" : Module.Replace('.', '/') + "/"; }
    }

    /// <summary>
    /// Gets the module slash, which is the module name with a trailing slash.
    /// Previously used for generating file paths and URLs, but now replaced by ModulePathPrefix and ModuleUrlPrefix.
    /// </summary>
    [Obsolete("Use ModulePathPrefix or ModuleUrlPrefix instead")]
    public string ModuleSlash
    {
        get { return string.IsNullOrEmpty(Module) ? "" : Module + "/"; }
    }

    /// <summary>
    /// Gets the module URL prefix, which is the module name with dots replaced by slashes and a trailing slash.
    /// If the module is null or empty, returns an empty string.
    /// </summary>
    public string ModuleUrlPrefix
    {
        get { return string.IsNullOrEmpty(Module) ? "" : Module.Replace('.', '/') + "/"; }
    }

    /// <summary>
    /// Gets the module dash, which is the module name with a trailing dash.
    /// </summary>
    [Obsolete("Left for backward compatibility")]
    public string ModuleDash
    {
        get { return string.IsNullOrEmpty(Module) ? "" : Module + "-"; }
    }

    /// <summary>
    /// Gets the schema dot, which is the schema name with a trailing dot.
    /// If the schema is null or empty, returns an empty string.
    /// </summary>
    public string SchemaDot
    {
        get { return string.IsNullOrEmpty(Schema) ? "" : Schema + "."; }
    }

    /// <summary>
    /// Gets the root namespace dot, which is the root namespace with a trailing dot.
    /// If the root namespace is null or empty, returns an empty string.
    /// </summary>
    public string RootNamespaceDot
    {
        get { return string.IsNullOrEmpty(RootNamespace) ? "" : RootNamespace + "."; }
    }

    /// <summary>
    /// Gets the root namespace dot followed by the module name.
    /// If the module is null or empty, returns just the root namespace dot.
    /// </summary>
    public string RootNamespaceDotModule
    {
        get { return RootNamespaceDot + (string.IsNullOrEmpty(Module) ? "" : Module); }
    }

    /// <summary>
    /// Gets the root namespace dot followed by the module name and a trailing dot.
    /// If the module is null or empty, returns just the root namespace dot.
    /// </summary>
    public string RootNamespaceDotModuleDot
    {
        get { return (string.IsNullOrEmpty(RootNamespaceDotModule) ? "" : RootNamespaceDotModule + "."); }
    }

    /// <summary>
    /// Gets the module namespace, which is the root namespace followed by the module name.
    /// </summary>
    public string ModuleNamespace => RootNamespaceDotModule;

    /// <summary>
    /// Gets the module namespace with a trailing dot, which is the root namespace followed by the module name and a dot.
    /// </summary>
    private string ModuleNamespaceDot => RootNamespaceDotModuleDot;

    /// <summary>
    /// Gets the handler namespace, which is the root namespace followed by the module name and ".RequestHandlers" if EnableGenerateInterface is true, otherwise just the root namespace followed by the module name.
    /// </summary>
    public string HandlerNamespace => EnableGenerateInterface ? (RootNamespaceDotModule + ".RequestHandlers") : RootNamespaceDotModule;

    /// <summary>
    /// Gets the row full name, which is the module namespace followed by the row class name.
    /// </summary>
    public string RowFullName => ModuleNamespaceDot + RowClassName;

    /// <summary>
    /// Gets the columns key, which is the module dot followed by the class name.
    /// </summary>
    public string ColumnsKey => ModuleDot + ClassName;

    /// <summary>
    /// Gets the columns namespace, which is the module namespace followed by "Columns".
    /// </summary>
    public string ColumnsNamespace => ModuleNamespaceDot + "Columns";

    /// <summary>
    /// Gets the columns class name, which is the class name followed by "Columns".
    /// </summary>
    public string ColumnsClassName => ClassName + "Columns";

    /// <summary>
    /// Gets the form namespace, which is the module namespace followed by "Forms".
    /// </summary>
    public string FormNamespace => ModuleNamespaceDot + "Forms";

    /// <summary>
    /// Gets the form key, which is the module dot followed by the class name.
    /// </summary>
    public string FormKey => ModuleDot + ClassName;

    /// <summary>
    /// Gets the form class name, which is the class name followed by "Form".
    /// </summary>
    public string FormClassName => ClassName + "Form";

    /// <summary>
    /// Gets the endpoint namespace, which is the module namespace followed by "Endpoints".
    /// </summary>
    public string EndpointNamespace => ModuleNamespaceDot + "Endpoints";

    /// <summary>
    /// Gets the endpoint class name, which is the class name followed by "Endpoint" if AspNetCore is true, otherwise followed by "Controller".
    /// </summary>
    public string EndpointClassName => ClassName + (AspNetCore ? "Endpoint" : "Controller");

    /// <summary>
    /// Gets the endpoint route template, which is "Services/" followed by the module URL segment, the class name, and "/[action]".
    /// </summary>
    public string EndpointRouteTemplate => "Services/" + ModuleUrlPrefix + ClassName + "/[action]";

    /// <summary>
    /// Gets the dialog class name, which is the class name followed by "Dialog".
    /// </summary>
    public string DialogClassName => ClassName + "Dialog";

    /// <summary>
    /// Gets the dialog full name, which is the module namespace followed by the dialog class name.
    /// </summary>
    public string DialogFullName => ModuleNamespaceDot + DialogClassName;

    /// <summary>
    /// Gets the grid class name, which is the class name followed by "Grid".
    /// </summary>
    public string GridClassName => ClassName + "Grid";

    /// <summary>
    /// Gets the grid full name, which is the module namespace followed by the grid class name.
    /// </summary>
    public string GridFullName => ModuleNamespaceDot + GridClassName;

    /// <summary>
    /// Gets the view page class name, which is the class name followed by "Page" if AspNetCore is true, otherwise followed by "Controller".
    /// </summary>
    public string ViewPageClassName => ClassName + (AspNetCore ? "Page" : "Controller");

    /// <summary>
    /// Gets the view page full name, which is the module namespace followed by the view page class name.
    /// </summary>
    public string ViewPageNamespace => ModuleNamespaceDot + "Pages";

    /// <summary>
    /// Gets the view page route, which is the module URL segment followed by the class name.
    /// </summary>
    public string ViewPageRoute => ModuleUrlPrefix + ClassName;

    /// <summary>
    /// Gets the view page route prefix, which is the module URL segment followed by the class name.
    /// </summary>
    public string ViewPageRoutePrefix => ModuleUrlPrefix + ClassName;

    /// <summary>
    /// Gets the view page module path, which is "@/" followed by the module name, the class name, and the class name followed by "Page".   
    /// </summary>
    public string ViewPageModulePath => "@/" + ModulePathPrefix + ClassName + "/" + ClassName + "Page";

    /// <summary>
    /// Gets the service class name, which is the class name followed by "Service".
    /// </summary>
    public string ServiceClassName => ClassName + "Service";

    /// <summary>
    /// Gets the service base URL, which is the module URL segment followed by the class name.
    /// </summary>
    public string ServiceBaseUrl => ModuleUrlPrefix + ClassName;

    /// <summary>
    /// Gets the entity singular text key, which is "Db" followed by the module name, the class name, and ".EntitySingular".
    /// </summary>
    public string EntityPluralTextKey => "Db" + DotModule + "." + ClassName + ".EntityPlural";

    /// <summary>
    /// Gets all fields, which is the concatenation of the regular fields and the join fields.
    /// </summary>
    public IEnumerable<EntityField> AllFields => Fields.Concat(JoinFields);

    /// <summary>
    /// Gets all join fields, which is the concatenation of the fields from all joins.
    /// </summary>
    public IEnumerable<EntityField> JoinFields => Joins.SelectMany(x => x.Fields);

    /// <summary>
    /// Gets the navigation category, which is the module name. This can be used for organizing entities in a navigation menu.
    /// </summary>
    public string? NavigationCategory
    {
        get { return Module; }
    }

    /// <summary>
    /// Gets the schema and table name in the format "[Schema].[TableName]" if schema is not empty, otherwise just returns the table name. This is used in the TableName attribute of the row class.
    /// </summary>
    public string SchemaAndTable
    {
        get { return string.IsNullOrEmpty(Schema) ? Tablename : "[" + Schema + "].[" + Tablename + "]"; }
    }
        
    /// <summary>
    /// Gets the row base class and interfaces as a comma-separated string.
    /// </summary>
    public string RowBaseClassAndInterfaces
    {
        get => string.Join(", ", RowBaseClassAndInterfaceList);
    }

    /// <summary>
    /// Represents an editor variable with its type and index. This is used to keep track of the distinct editor types used in the entity's fields and their corresponding indices.
    /// </summary>
    /// <param name="Editor">The type of the editor.</param>
    /// <param name="Index">The index of the editor.</param>
    public record EditorVariable(string Editor, int Index);

    private List<EditorVariable> editorVariables = null!;

    /// <summary>
    /// Gets the list of distinct editor variables used in the entity's fields. Each editor variable contains the editor type and its corresponding index. This is useful for generating client-side code that needs to reference the editors used in the entity's fields.
    /// </summary>
    public List<EditorVariable> EditorVariables
    {
        get
        {
            if (editorVariables.IsEmptyOrNull())
                editorVariables = [.. Fields.Select((x) => x.TSEditorType).Distinct().Select((x, i) => new EditorVariable(x, i))];
            return editorVariables;
        }
    }

    /// <summary>
    /// Gets the list of row base class and interfaces. The first item is the row base class.
    /// </summary>
    public List<string> RowBaseClassAndInterfaceList
    {
        get
        {
            var result = new List<string> { RowBaseClass ?? "Serenity.Data.Row" };

            if (!string.IsNullOrEmpty(IdField))
                result.Add("Serenity.Data.IIdRow");
            if (!string.IsNullOrEmpty(NameField))
                result.Add("Serenity.Data.INameRow");

            return result;
        }
    }
}