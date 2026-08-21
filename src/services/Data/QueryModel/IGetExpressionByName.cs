namespace Serenity.Data;

/// <summary>
///   Interface for query classes (e.g. SqlQuery) that can return expression given column name.
/// </summary>
public interface IGetExpressionByName
{
    /// <summary>
    /// Gets the expression.
    /// </summary>
    /// <param name="columnName">Name of the column.</param>
    /// <returns>The expression for the column, or <c>null</c> if not found.</returns>
    string GetExpression(string columnName);
}
