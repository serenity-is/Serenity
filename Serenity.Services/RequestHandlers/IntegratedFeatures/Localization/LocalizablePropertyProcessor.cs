using Serenity.ComponentModel;
using Serenity.Data;
using System;
using System.ComponentModel;
using System.Reflection;

namespace Serenity.PropertyGrid
{
    public partial class LocalizablePropertyProcessor : PropertyProcessor
    {
        private ILocalizationRowHandler localizationRowHandler;

        public override void Initialize()
        {
            if (BasedOnRow == null)
                return;

            var attr = BasedOnRow.GetType().GetCustomAttribute<LocalizationRowAttribute>(false);

            if (attr != null)
                localizationRowHandler = Activator.CreateInstance(typeof(LocalizationRowHandler<>)
                    .MakeGenericType(BasedOnRow.GetType())) as ILocalizationRowHandler;
        }

        public override void Process(IPropertySource source, PropertyItem item)
        {
            var attr = source.GetAttribute<LocalizableAttribute>();
            if (attr != null)
            {
                if (item.IsLocalizable)
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

        public override int Priority
        {
            get { return 15; }
        }
    }
}
