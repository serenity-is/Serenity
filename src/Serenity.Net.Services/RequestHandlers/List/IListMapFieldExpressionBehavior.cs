namespace Serenity.Services;

/// <summary>
/// An extension for list behaviors that can map fields to custom expressions 
/// within a ListRequestHandler lifecycle. Note that the first behavior
/// that returns a non-null value will be used.
/// </summary>
public interface IListMapFieldExpressionBehavior
{
    /// <summary>Maps field's expression to a custom one. 
    /// Returns null if not mapped to a custom expression.</summary>
    /// <param name="handler">List handler</param>
    /// <param name="query">Query</param>
    /// <param name="field">Field</param>
    string MapFieldExpression(IListRequestHandler handler, SqlQuery query, IField field);
}