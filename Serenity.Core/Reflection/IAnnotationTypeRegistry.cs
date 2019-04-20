using System;
using System.Collections.Generic;
using System.Reflection;

namespace Serenity.Reflection
{
    /// <summary>
    /// An interface to query list of annotation types for a given type
    /// </summary>
    public interface IAnnotationTypeRegistry
    {
        IEnumerable<Type> GetAnnotationTypesFor(Type type);
        IAnnotatedType GetAnnotatedType(Type type);
        void Reset();
    }
}