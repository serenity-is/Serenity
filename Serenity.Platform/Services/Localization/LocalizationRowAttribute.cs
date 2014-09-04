using System;

namespace Serenity.Data
{
    public class LocalizationRowAttribute : Attribute
    {
        public LocalizationRowAttribute(string localizationTable, string mappedIdField)
        {
            this.LocalizationTable = localizationTable;
            this.MappedIdField = mappedIdField;
        }

        public string LocalizationTable { get; private set; }
        public string MappedIdField { get; private set; }
    }
}