using Serenity.ComponentModel;
using Serenity.Data;
using System.Linq;

namespace Serenity.PropertyGrid
{
    public partial class BasicPropertyProcessor : PropertyProcessor
    {
        private void SetEditLink(IPropertySource source, PropertyItem item)
        {
            var attr = source.GetAttribute<EditLinkAttribute>();
            if (attr == null)
                return;

            if (attr.Value)
                item.EditLink = true;

            if (attr.ItemType != null)
                item.EditLinkItemType = attr.ItemType;

            if (attr.IdField != null)
                item.EditLinkIdField = attr.IdField;

            if (attr.CssClass != null)
                item.EditLinkCssClass = attr.CssClass;

            if (item.EditLinkItemType != null &&
                item.EditLinkIdField == null)
            {
                item.EditLinkIdField = AutoDetermineIdField(source.BasedOnField);
            }
        }

        private static string AutoDetermineIdField(Field basedOnField)
        {
            if (ReferenceEquals(null, basedOnField) || basedOnField.Join == null)
                return null;

            var idField = basedOnField.Fields.FirstOrDefault(x => x.ForeignJoinAlias != null &&
                x.ForeignJoinAlias.Name == basedOnField.Join.Name);

            return ReferenceEquals(null, idField) ? null : (idField.PropertyName ?? idField.Name);
        }
    }
}