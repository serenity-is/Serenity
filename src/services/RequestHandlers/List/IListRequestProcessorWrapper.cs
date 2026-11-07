namespace Serenity.Services;

/// <summary>
/// Implemented by the sync / async list request processor wrappers, so that
/// a capability of the wrapped handler, like <see cref="IListQueryCapture"/>,
/// can be found through them.
/// </summary>
internal interface IListRequestProcessorWrapper
{
    /// <summary>
    /// Gets the wrapped handler.
    /// </summary>
    IListRequestHandler WrappedHandler { get; }
}
