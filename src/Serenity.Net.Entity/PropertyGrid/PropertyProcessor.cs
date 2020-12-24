using Serenity.ComponentModel;
using Serenity.Data;
using System;
using System.Collections.Generic;

namespace Serenity.PropertyGrid
{
    public abstract class PropertyProcessor : IPropertyProcessor
    {
        public virtual void Process(IPropertySource source, PropertyItem item)
        {
        }

        public virtual void Initialize()
        {
        }

        public virtual void PostProcess()
        {
        }

        public virtual int Priority => 50;

        public Type Type { get; set; }
        public IRow BasedOnRow { get; set; }
        public List<PropertyItem> Items { get; set; }
    }
}