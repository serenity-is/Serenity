using System;

namespace Serenity.Data
{
    public class LocalizationRowAttribute : Attribute
    {
        public LocalizationRowAttribute(Type localizationRow)
        {
            LocalizationRow = localizationRow ?? throw new ArgumentNullException("localizationRow");
        }

        public Type LocalizationRow { get; private set; }
        public string MappedIdField { get; set; }
    }
}