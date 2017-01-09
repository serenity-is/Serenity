using System;

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