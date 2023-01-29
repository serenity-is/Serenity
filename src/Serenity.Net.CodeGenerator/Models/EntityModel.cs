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
    public string RowBaseClass { get; set; }
    public List<EntityField> RowBaseFields { get; set; }
    public string FieldsBaseClass { get; set; }
    public bool IsLookup { get; set; }
    public List<EntityField> Fields { get; set; }
    public List<EntityJoin> Joins { get;set;}
    public bool Instance { get; set; }
    public string NameField { get; set; }
    public string FieldPrefix { get; set; }
    public bool AspNetCore { get; set; }
    public bool NET5Plus { get; set; }

    public string IdField { get { return Identity; } }
    public Dictionary<string, object> CustomSettings { get; set; }

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