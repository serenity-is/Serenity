namespace Serenity.Services;

/// <summary>
/// Extension methods for behavior provider
/// </summary>
public static class BehaviorProviderExtensions
{
    /// <summary>
    /// Resolves behaviors for handler, row and behavior type
    /// </summary>
    /// <typeparam name="TRow">Row type</typeparam>
    /// <typeparam name="TBehavior">Behavior type</typeparam>
    /// <param name="provider">Provider</param>
    /// <param name="handlerType">Handler type</param>
    /// <returns>Behavior</returns>
    public static IEnumerable<TBehavior> Resolve<TRow, TBehavior>(this IBehaviorProvider provider, Type handlerType)
    {
        return provider.Resolve(handlerType, typeof(TRow), typeof(TBehavior)).Cast<TBehavior>();
    }
}