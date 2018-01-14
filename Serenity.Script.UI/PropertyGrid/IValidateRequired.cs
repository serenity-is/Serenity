using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true)]
    public interface IValidateRequired
    {
        bool Required { get; set; }
    }
}