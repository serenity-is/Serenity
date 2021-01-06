using Serenity.ComponentModel;
using Serenity.Data;
using System;
using System.Collections.Generic;

namespace Serenity.PropertyGrid
{
    public interface IPropertyProcessor
    {
        void Initialize();
        void Process(IPropertySource source, PropertyItem item);
        void PostProcess();
        List<PropertyItem> Items { get; set; }
        Type Type { get; set; }
        IRow BasedOnRow { get; set; }
        int Priority { get; }
    }
}