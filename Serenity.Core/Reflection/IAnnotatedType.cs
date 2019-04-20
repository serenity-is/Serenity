using System.Reflection;

namespace Serenity.Reflection
{
    public interface IAnnotatedType
    {
        IPropertyInfo GetAnnotatedProperty(PropertyInfo property);
    }
}