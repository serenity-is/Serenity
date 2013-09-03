using System;

namespace Serenity.Data
{
    public class DataEnumAttribute
    {
        public DataEnumAttribute(string schema)
        {
            this.Schema = schema;
        }

        public string Schema { get; private set; }
    }
}