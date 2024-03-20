namespace Serenity.TypeScript;

public class JSDocArray : List<JSDoc>
{
    public JSDocArray()
    {
    }

    public JSDocArray(JSDoc[] elements)
        : base([.. elements])
    {
    }

    public List<IJSDocTag> JSDocCache { get; set; }
}