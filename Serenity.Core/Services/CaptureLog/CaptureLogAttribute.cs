using System;

namespace Serenity.Data
{
    public class CaptureLogAttribute : Attribute
    {
        public CaptureLogAttribute(string logTable, string mappedIdField)
        {
            this.LogTable = logTable;
            this.MappedIdField = mappedIdField;
        }

        public string LogTable { get; private set; }
        public string MappedIdField { get; private set; }
    }
}