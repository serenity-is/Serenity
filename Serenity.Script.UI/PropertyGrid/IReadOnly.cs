using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true)]
    public interface IReadOnly
    {
        bool ReadOnly { get; set; }
    }
}