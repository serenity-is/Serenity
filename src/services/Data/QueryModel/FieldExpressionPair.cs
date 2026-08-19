namespace Serenity.Data;

/// <summary>A pair of a field name and its SQL value expression.</summary>
public readonly record struct FieldExpressionPair(string Field, string Expression);