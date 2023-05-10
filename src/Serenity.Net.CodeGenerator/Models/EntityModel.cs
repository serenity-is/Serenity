namespace Serenity.CodeGenerator;

public class EntityModel
{
    public string Module { get; set; }
    public string ConnectionKey { get; set; }
    public string Permission { get; set; }
    public string RootNamespace { get; set; }
    public string ClassName { get; set; }
    public string RowClassName { get; set; }
    public string Schema { get; set; }
    public string Tablename { get; set; }
    public string Title { get; set; }
    public string Identity { get; set; }
    public string RowBaseClass { get; set; } = "Serenity.Data.Row";
    public List<EntityField> RowBaseFields { get; } = new();
    public string FieldsBaseClass { get; set; } = "Serenity.Data.RowFieldsBase";
    public bool IsLookup { get; set; }
    public List<EntityField> Fields { get; } = new();
    public List<EntityJoin> Joins { get; } = new();
    public string NameField { get; set; }
    public string FieldPrefix { get; set; }
    public bool AspNetCore { get; set; } = true;
    public bool NET5Plus { get; set; } = true;
    public bool DeclareJoinConstants { get; set; }
    public bool EnableRowTemplates { get; set; }
    public bool FileScopedNamespaces { get; set; }
    public HashSet<string> GlobalUsings { get; } = new();

    public string IdField => Identity;
    public Dictionary<string, object> CustomSettings { get; set; }

    public IEnumerable<EntityField> FormFields => Fields.Where(f => !f.OmitInForm);
    public IEnumerable<EntityField> GridFields => Fields.Where(f => !f.OmitInGrid);

    public string DotModule
    {
        get { return string.IsNullOrEmpty(Module) ? "" : "." + Module; }
    }

    public string ModuleDot
    {
        get { return string.IsNullOrEmpty(Module) ? "" : Module + "."; }
    }

    public string ModuleSlash
    {
        get { return string.IsNullOrEmpty(Module) ? "" : Module + "/"; }
    }

    public string ModuleDash
    {
        get { return string.IsNullOrEmpty(Module) ? "" : Module + "-"; }
    }

    public string SchemaDot
    {
        get { return string.IsNullOrEmpty(Schema) ? "" : Schema + "."; }
    }

    public string RootNamespaceDot
    {
        get { return string.IsNullOrEmpty(RootNamespace) ? "" : RootNamespace + "."; }
    }

    public string RootNamespaceDotModule
    {
        get { return RootNamespaceDot + (string.IsNullOrEmpty(Module) ? "" : Module); }
    }

    public string RootNamespaceDotModuleDot
    {
        get { return (string.IsNullOrEmpty(RootNamespaceDotModule) ? "" : RootNamespaceDotModule + "."); }
    }

    public string ModuleNamespace => RootNamespaceDotModule;
    private string ModuleNamespaceDot => RootNamespaceDotModuleDot;

    public string RowFullName => ModuleNamespaceDot + RowClassName;

    public string ColumnsKey => ModuleDot + ClassName;
    public string ColumnsNamespace => ModuleNamespaceDot + "Columns";
    public string ColumnsClassName => ClassName + "Columns";
    public string FormNamespace => ModuleNamespaceDot + "Forms";
    public string FormKey => ModuleDot + ClassName;
    public string FormClassName => ClassName + "Form";
    public string EndpointNamespace => ModuleNamespaceDot + "Endpoints";
    public string EndpointClassName => ClassName + (AspNetCore ? "Endpoint" : "Controller");
    public string EndpointRouteTemplate => "Services/" + ModuleSlash + ClassName + "/[action]";
    public string DialogClassName => ClassName + "Dialog";
    public string DialogFullName => ModuleNamespaceDot + DialogClassName;
    public string GridClassName => ClassName + "Grid";
    public string GridFullName => ModuleNamespaceDot + GridClassName;
    public string ViewPageClassName => ClassName + (AspNetCore ? "Page" : "Controller");
    public string ViewPageNamespace => ModuleNamespaceDot + "Pages";
    public string ViewPageRoute => ModuleSlash + ClassName;
    public string ViewPageRoutePrefix => ModuleSlash + ClassName;
    public string ViewPageIndexPath => "~/Modules/" + ModuleSlash + ClassName + "/" + ClassName + "Index.cshtml";
    public string ViewPageModulePath => "@/" + Module + "/" + ClassName + "/" + ClassName + "Page";
    public string ServiceClassName => ClassName + "Service";
    public string ServiceBaseUrl => ModuleSlash + ClassName;
    public string EntityPluralTextKey => "Db" + DotModule + ClassName + ".EntityPlural";

    public IEnumerable<EntityField> AllFields => Fields.Concat(JoinFields);
    public IEnumerable<EntityField> JoinFields => Joins.SelectMany(x => x.Fields);

    public string NavigationCategory
    {
        get { return Module; }
    }

    public string SchemaAndTable
    {
        get { return string.IsNullOrEmpty(Schema)? Tablename : "[" + Schema + "].[" + Tablename + "]"; }
    }

    public string RowBaseClassAndInterfaces
    {
        get => string.Join(", ", RowBaseClassAndInterfaceList);
    }

    public record EditorVariable(string Editor, int Index);

    private List<EditorVariable> editorVariables;

    public List<EditorVariable> EditorVariables
    {
        get
        {
            if (editorVariables.IsEmptyOrNull())
                editorVariables = Fields.Select((x) => x.TSEditorType).Distinct().Select((x, i) => new EditorVariable(x, i)).ToList();
            return editorVariables;
        }
    }
    
    public List<string> RowBaseClassAndInterfaceList
    {
        get
        {
            var result = new List<string> { RowBaseClass ?? "Serenity.Data.Row" };

            if (!string.IsNullOrEmpty(Identity))
                result.Add("Serenity.Data.IIdRow");
            if (!string.IsNullOrEmpty(NameField))
                result.Add("Serenity.Data.INameRow");

            return result;
        }
    }
}