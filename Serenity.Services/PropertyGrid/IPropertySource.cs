using Newtonsoft.Json;
using Serenity.ComponentModel;
using Serenity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Linq;

namespace Serenity.PropertyGrid
{
    public interface IPropertySource
    {
        PropertyInfo Property { get; }
        Field BasedOnField { get; }
        TAttr GetAttribute<TAttr>() where TAttr : Attribute;
        IEnumerable<TAttr> GetAttributes<TAttr>() where TAttr : Attribute;
        Type ValueType { get; }
        Type EnumType { get; }
    }
}