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

    /// <summary>
    /// Takes a list of behaviors implementing a common marker interface <typeparamref name="TBase"/>,
    /// and converts it to a list of behaviors implementing <typeparamref name="TNative"/>.
    /// Behaviors already implementing <typeparamref name="TNative"/> are returned as is, while behaviors
    /// implementing only the source variant <typeparamref name="TSource"/> are wrapped using the
    /// <paramref name="wrap"/> factory function.
    /// </summary>
    /// <typeparam name="TBase">Common base (marker) interface, e.g. <see cref="ISaveBehavior"/></typeparam>
    /// <typeparam name="TSource">The source variant interface, e.g. <see cref="ISaveBehaviorAsync"/> when
    /// wrapping to sync</typeparam>
    /// <typeparam name="TNative">The target variant interface, e.g. <see cref="ISaveBehaviorSync"/></typeparam>
    /// <param name="input">List of behaviors</param>
    /// <param name="wrap">Factory to wrap a behavior of type <typeparamref name="TSource"/> into one of type
    /// <typeparamref name="TNative"/></param>
    /// <returns>List of behaviors implementing <typeparamref name="TNative"/></returns>
    /// <exception cref="InvalidOperationException">One of the behaviors implements neither
    /// <typeparamref name="TSource"/> nor <typeparamref name="TNative"/>.</exception>
    public static IEnumerable<TNative> AutoWrapBehaviors<TBase, TSource, TNative>(
        IEnumerable<TBase> input, Func<TSource, TNative> wrap)
        where TNative : TBase
        where TSource : TBase
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(wrap);

        foreach (var behavior in input)
        {
            if (behavior is TNative nativeBehavior)
                yield return nativeBehavior;
            else if (behavior is TSource sourceBehavior)
                yield return wrap(sourceBehavior);
            else
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                    "'{0}' type implements {1} but does not implement {2} or {3}! Please implement one of these " +
                    "interfaces, or derive from a class like Base(Save/Delete/List)Behavior that implements one of them.",
                    behavior?.GetType().FullName ?? "<null>",
                    typeof(TBase).Name, typeof(TNative).Name, typeof(TSource).Name));
        }
    }
}