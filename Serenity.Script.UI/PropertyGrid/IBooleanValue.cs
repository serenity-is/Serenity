
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true)]
    public interface IBooleanValue
    {
        bool Value { get; set; }
    }
}