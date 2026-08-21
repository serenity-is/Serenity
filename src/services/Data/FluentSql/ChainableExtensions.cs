namespace Serenity;

/// <summary>
/// Contains method chaining extensions for objects like SQL queries that implement
/// <see cref="IChainable"/>.
/// </summary>
public static class ChainableExtensions
{
    /// <summary>
    /// Allows to reference the call chain object itself without breaking a call chain.
    /// </summary>
    /// <param name="chain">The chaining object.</param>
    /// <param name="action">An action that will be called with the chain object as parameter.</param>
    /// <returns>
    /// The query itself.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
    public static TChain With<TChain>(this TChain chain, Action<TChain> action)
        where TChain : IChainable
    {
        ArgumentNullException.ThrowIfNull(action);

        action(chain);

        return chain;
    }

}