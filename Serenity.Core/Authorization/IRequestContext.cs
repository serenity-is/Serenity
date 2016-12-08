using System.Collections;

namespace Serenity.Abstractions
{
    /// <summary>
    /// Interface for abstraction that should return HttpContext.Current.Items for web requests,
    /// and null for normal threads.
    /// </summary>
    public interface IRequestContext
    {
        /// <summary>
        /// A dictionary that can be used as a request context specific storage
        /// </summary>
        IDictionary Items { get; }
    }
}