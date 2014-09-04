using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.ComponentModel;
using Serenity.Data;

namespace Serenity.Data
{
    public class InstanceNameAttribute : Attribute
    {
        public InstanceNameAttribute(string instanceName)
        {
            this.InstanceName = instanceName;
        }

        public string InstanceName { get; private set; }
    }
}