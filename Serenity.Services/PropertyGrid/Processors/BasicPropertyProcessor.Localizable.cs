using Serenity.ComponentModel;
using Serenity.Data;
using System;
using System.ComponentModel;
using System.Reflection;

namespace Serenity.PropertyGrid
{
    public partial class BasicPropertyProcessor : PropertyProcessor
    {
        private ILocalizationRowHandler localizationRowHandler;

        private void SetLocalizable(IPropertySource source, PropertyItem item)
        {
            var attr = source.GetAttribute<LocalizableAttribute>();
            if (attr != null)
            {
                item.Localizable = true;
                return;
            }

            if (!ReferenceEquals(null, source.BasedOnField) &&
                localizationRowHandler != null &&
                localizationRowHandler.IsLocalized(source.BasedOnField))
            {
                item.Localizable = true;
            }
        }

        private void InitLocalizable()
        {
            if (BasedOnRow == null)
                return;

            var attr = BasedOnRow.GetType().GetCustomAttribute<LocalizationRowAttribute>(false);

            if (attr != null)
                localizationRowHandler = Activator.CreateInstance(typeof(LocalizationRowHandler<>)
                    .MakeGenericType(BasedOnRow.GetType())) as ILocalizationRowHandler;
        }

    }
}