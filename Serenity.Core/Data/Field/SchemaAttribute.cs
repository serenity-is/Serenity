using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.ComponentModel;
using Serenity.Data;

namespace Serenity.Data
{
    public class SchemaAttribute : Attribute
    {
        public SchemaAttribute(string schema)
        {
            this.Schema = schema;
        }

        public string Schema { get; private set; }
    }
}