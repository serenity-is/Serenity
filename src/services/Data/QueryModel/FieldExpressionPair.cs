namespace Serenity.Data;

/// <summary>A pair of a field name and its SQL value expression.</summary>
/// <param name="Field">The field name.</param>
/// <param name="Expression">The SQL value expression.</param>
public readonly record struct FieldExpressionPair(string Field, string Expression);