using System;

namespace Serenity.Data
{
    public class LocalizationRowAttribute : Attribute
    {
        public LocalizationRowAttribute(Type localizationRow)
        {
            this.LocalizationRow = localizationRow;
        }

        public Type LocalizationRow { get; private set; }
        public string MappedIdField { get; private set; }
    }
}