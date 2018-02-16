using System;

namespace Serenity.ComponentModel
{
    public class ModuleAttribute : Attribute
    {
        public ModuleAttribute(string module)
        {
            this.Value = module;
        }

        public string Value { get; private set; }
    }
}