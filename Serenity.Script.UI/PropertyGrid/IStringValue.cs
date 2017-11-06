
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true)]
    public interface IStringValue
    {
        string Value { get; set; }
    }
}