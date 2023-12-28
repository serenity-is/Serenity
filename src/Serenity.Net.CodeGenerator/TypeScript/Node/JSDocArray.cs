namespace Serenity.TypeScript;

internal class JSDocArray : List<JSDoc>
{
    public JSDocArray()
    {
    }

    public JSDocArray(JSDoc[] elements)
        : base(elements.ToList())
    {
    }

    public List<IJSDocTag> JSDocCache { get; set; }
}
