
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true)]
    public interface ISetEditValue
    {
        void SetEditValue(dynamic source, PropertyItem property);
    }
}