namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private void SetEditorAddons(IPropertySource source, PropertyItem item)
    {
        var attrs = source.GetAttributes<EditorAddonAttribute>();
        if (!attrs.Any())
            return;

        item.EditorAddons = attrs.Select(x =>
        {
            var addon = new EditorAddonItem
            {
                Type = x.AddonType
            };
            var p = new Dictionary<string, object>();
            x.SetParams(p);
            if (p.Count > 0)
                addon.Params = p;
            return addon;
        }).ToList();
    }
}