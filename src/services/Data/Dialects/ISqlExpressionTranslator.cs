namespace Serenity.Data;

/// <summary>
/// An interface that custom <see cref="ISqlDialect"/> types can implement to change the
/// generated expressions for dynamic expressions like <c>ConcatExpressionAttribute</c>.
/// Implementors should return <c>null</c> if they cannot handle the given expression type.
/// </summary>
public interface ISqlExpressionTranslator
{
    /// <summary>
    /// Returns a customized version for the passed expression attribute.
    /// If the dialect has no custom handling for the expression, it should return <c>null</c>.
    /// </summary>
    /// <param name="expression">The expression attribute or a subclass.
    /// In some cases it can be another unknown object type,
    /// so the implementor should check the type.</param>
    /// <returns>The translated expression, or <c>null</c> if the dialect cannot handle it.</returns>
    string Translate(object expression);
}
