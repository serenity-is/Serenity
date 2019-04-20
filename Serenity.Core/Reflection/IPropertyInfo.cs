using System;
using System.Collections.Generic;
using System.Reflection;

namespace Serenity.Reflection
{
    /// <summary>
    /// An interface to virtualize property attribute access
    /// </summary>
    public interface IPropertyInfo
    {
        string Name { get; }
        TAttr GetAttribute<TAttr>() where TAttr : Attribute;
        IEnumerable<TAttr> GetAttributes<TAttr>() where TAttr : Attribute;
        Type PropertyType { get; }
    }
}