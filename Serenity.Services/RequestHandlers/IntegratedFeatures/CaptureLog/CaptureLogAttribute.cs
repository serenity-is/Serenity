using System;

namespace Serenity.Data
{
    public class CaptureLogAttribute : Attribute
    {
        public CaptureLogAttribute(Type logRow)
        {
            this.LogRow = logRow;
        }

        public Type LogRow { get; private set; }
        public string MappedIdField { get; set; }
    }
}