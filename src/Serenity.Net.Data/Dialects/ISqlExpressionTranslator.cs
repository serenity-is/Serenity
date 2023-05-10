namespace Serenity.Data;

/// <summary>
/// An interface custom ISqlDialect types can implement to change generated 
/// expressions for dynamic expression like ConcatExpressionAttribute.
/// They should return NULL if it can't handle this expression type
/// </summary>
public interface ISqlExpressionTranslator
{
    /// <summary>
    /// Returns customized version for passed expression attribute.
    /// If the dialect don't have custom handling for the expression,
    /// it should return null.
    /// </summary>
    /// <param name="expression">Expression attribute or subclass.
    /// In some cases it can be another unknown object type
    /// so the implementor should check the type.</param>
    string Translate(object expression);
}
