namespace Serenity.Data;

/// <summary>
/// Thrown when infinite recursion is detected while resolving [Origin] attributes.
/// </summary>
internal sealed class OriginRecursionException() : Exception("Infinite origin recursion detected!");
