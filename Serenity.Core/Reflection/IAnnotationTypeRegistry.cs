using System;
using System.Collections.Generic;

namespace Serenity.Reflection
{
    /// <summary>
    /// An interface to query list of annotation types for a given type
    /// </summary>
    public interface IAnnotationTypeRegistry
    {
        /// <summary>
        /// Gets the annotation types for given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        IEnumerable<Type> GetAnnotationTypesFor(Type type);

        /// <summary>
        /// Resets this instance.
        /// </summary>
        void Reset();
    }
}