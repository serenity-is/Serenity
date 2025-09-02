namespace Serenity;

/// <summary>
/// Contains method chaining extensions for objects like SQL queries that implement
/// <see cref="IChainable"/>
/// </summary>
public static class ChainableExtensions
{
    /// <summary>
    /// Allows to reference the call chain object itself without breaking a call chain.
    /// </summary>
    /// <param name="chain">Chaining object</param>
    /// <param name="action">An action that will be called with the chain object as parameter.</param>
    /// <returns>
    /// The query itself.</returns>
    public static TChain With<TChain>(this TChain chain, Action<TChain> action)
        where TChain : IChainable
    {
        if (action == null)
            throw new ArgumentNullException("action");

        action(chain);

        return chain;
    }

}