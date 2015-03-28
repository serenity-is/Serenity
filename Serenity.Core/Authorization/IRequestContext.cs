using System.Collections;

namespace Serenity.Abstractions
{
    /// <summary>
    /// Interface for abstraction that should return HttpContext.Current.Items for web requests,
    /// and null for normal threads.
    /// </summary>
    public interface IRequestContext
    {
        IDictionary Items { get; }
    }
}