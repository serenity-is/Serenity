
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true)]
    public interface IGetEditValue
    {
        void GetEditValue(PropertyItem property, dynamic target);
    }
}