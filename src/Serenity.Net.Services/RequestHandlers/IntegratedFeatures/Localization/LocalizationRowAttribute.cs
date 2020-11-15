using System;

namespace Serenity.Data
{
    public class LocalizationRowAttribute : Attribute
    {
        public LocalizationRowAttribute(Type localizationRow)
        {
            if (localizationRow == null)
                throw new ArgumentNullException("localizationRow");

            this.LocalizationRow = localizationRow;
        }

        public Type LocalizationRow { get; private set; }
        public string MappedIdField { get; set; }
    }
}