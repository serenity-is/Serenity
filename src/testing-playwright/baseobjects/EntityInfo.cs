namespace Serenity.Testing.Playwright;

public record EntityInfo(Type Type)
{
    protected EntityInfo(EntityInfo other)
    {
        Type = other.Type;
        displayName = other.displayName;
        identifier = other.identifier;
        itemName = other.itemName;
        fullName = other.fullName;
        gridTitle = other.gridTitle;
        moduleKey = other.moduleKey;
        @namespace = other.@namespace;
        pageUrl = other.pageUrl;
        windowTitle = other.windowTitle;
    }

    private string displayName;
    private string identifier;
    private string itemName;
    private string fullName;
    private string gridTitle;
    private string moduleKey;
    private string @namespace;
    private string pageUrl;
    private string windowTitle;

    public string Identifier
    {
        get => identifier ?? (Type.Name.EndsWith("Grid") ?
            Type.Name[0..^4] : (Type.Name.EndsWith("Dialog") ?
                Type.Name[0..^6] : Type.Name));
        set => identifier = value;
    }

    public string ModuleKey
    {
        get => moduleKey ?? Type.Namespace switch
        {
            "StartSharp.Tests.BasicSamples" => "BasicSamples",
            "StartSharp.Tests.Northwind" => "Northwind",
            "StartSharp.Tests.AdvancedSamples" => "AdvancedSamples",
            _ => Type.FullName.Split('.').LastOrDefault() ?? "__UnknownModuleName__"
        };
        set => moduleKey = value;
    }

    public string ItemName
    {
        get => itemName ?? Identifier;
        set => itemName = value;
    }

    public string DisplayName
    {
        get => displayName ?? ItemName.SimplePluralize();
        set => displayName = value;
    }

    public string FullName
    {
        get => Namespace + "." + GetType().Name;
        set => fullName = value;
    }

    public string GridTitle
    {
        get => gridTitle ?? DisplayName;
        set => gridTitle = value;
    }

    public string WindowTitle
    {
        get => windowTitle ?? DisplayName;
        set => windowTitle = value;
    }

    public string Namespace
    {
        get => @namespace ?? Type.Namespace switch
        {
            "StartSharp.Tests.BasicSamples" => "Serenity.Demo.BasicSamples",
            "StartSharp.Tests.Northwind" => "Serenity.Demo.Northwind",
            "StartSharp.Tests.AdvancedSamples" => "Serenity.Demo.AdvancedSamples",
            _ => ModuleKey
        };
        set => @namespace = value;
    }

    public string PageUrl
    {
        get => pageUrl ?? ModuleKey.Replace('.', '/') + "/" + Identifier + PageUrlSuffix;
        set => pageUrl = value;
    }

    public string PageUrlSuffix { get; set; } = "";
}