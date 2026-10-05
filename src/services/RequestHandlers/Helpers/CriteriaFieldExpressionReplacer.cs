using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Serenity.Data;

/// <summary>
/// Converts field names in a criteria to their
/// corresponding SQL field expressions.
/// </summary>
/// <remarks>
/// Initializes a new instance of the class.
/// </remarks>
/// <param name="row">The row instance</param>
/// <param name="permissions">Permission service</param>
/// <param name="lookupAccessMode">Use lookup access mode.
/// In the lookup access mode only the lookup fields can be
/// used in the filter. Default is false.</param>
/// <param name="dialect">Optional dialect</param>
/// <param name="toCriteria">Optional field to criteria converter</param>
/// <param name="allowFilterField">Optional field filter validator. If provided, it will be used to determine if a field can be filtered
/// instead of the default logic using see <see cref="permissions"/> argument.</param>
/// <exception cref="ArgumentNullException"><paramref name="row"/> or <paramref name="permissions"/> is <c>null</c>.</exception>
public class CriteriaFieldExpressionReplacer(IRow row, IPermissionService permissions,
    bool lookupAccessMode = false, ISqlDialect? dialect = null, Func<IField, BaseCriteria>? toCriteria = null,
    Func<Field, bool>? allowFilterField = null) : SafeCriteriaValidator
{
    private readonly IPermissionService permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
    private readonly bool lookupAccessMode = lookupAccessMode;
    private readonly Func<Field, bool>? allowFilterField = allowFilterField;

    /// <summary>
    /// Gets the row instance.
    /// </summary>
    protected IRow Row { get; private set; } = row ?? throw new ArgumentNullException(nameof(row));

    /// <summary>
    /// Gets the dialect passed in or the default dialect.
    /// </summary>
    protected ISqlDialect Dialect { get; } = dialect ?? SqlSettings.DefaultDialect;

    /// <summary>
    /// Visits the criteria for conversion and returns
    /// a processed criteria containing replaced field
    /// expressions.
    /// </summary>
    /// <param name="criteria">The criteria</param>
    [return: NotNullIfNotNull(nameof(criteria))]
    public BaseCriteria? Process(BaseCriteria? criteria)
    {
        return Visit(criteria);
    }

    /// <summary>
    /// Virtual method to check if a Field can be filtered.
    /// </summary>
    /// <param name="field">Field instance</param>
    protected virtual bool CanFilterField(Field field)
    {
        if (field.Flags.HasFlag(FieldFlags.NotMapped))
            return false;

        if (allowFilterField is not null)
            return allowFilterField(field);

        return IsFieldFilterAllowed(field, permissions, lookupAccessMode);
    }

    /// <summary>
    /// Returns whether filtering a field is allowed by its flags and permissions.
    /// </summary>
    /// <param name="field">The field to check.</param>
    /// <param name="permissions">The permission service.</param>
    /// <param name="lookupAccessMode">Whether lookup access mode is enabled.</param>
    /// <returns>True if filtering the field is allowed.</returns>
    public static bool IsFieldFilterAllowed(Field field, IPermissionService permissions,
        bool lookupAccessMode = false)
    {
        if (field.Flags.HasFlag(FieldFlags.DenyFiltering))
            return false;

        if (field.MinSelectLevel == SelectLevel.Never)
            return false;

        if (field.ReadPermission != null &&
            !permissions.HasPermission(field.ReadPermission))
            return false;

        if (field.ReadPermission == null &&
            lookupAccessMode &&
            !field.IsLookup)
            return false;

        return true;
    }

    /// <summary>
    /// Finds a field by its property name or field name
    /// </summary>
    /// <param name="expression">The property name or field name</param>
    protected virtual Field? FindField(string expression)
    {
        return Row.FindFieldByPropertyName(expression) ?? Row.FindField(expression);
    }

    /// <summary>
    /// Converts field to criteria
    /// </summary>
    /// <param name="field">Field</param>
    protected virtual BaseCriteria ToCriteria(IField field)
    {
        return toCriteria != null ? toCriteria(field) : new Criteria(field);
    }

    /// <inheritdoc/>
    protected override BaseCriteria VisitCriteria(Criteria criteria)
    {
        if (string.IsNullOrEmpty(criteria.Expression))
            throw new ValidationError("InvalidCriteriaField", null,
                "Empty criteria field name is not allowed!");

        var result = base.VisitCriteria(criteria);

        if (result is Criteria critResult)
        {
            var field = FindField(critResult.Expression) ?? throw new ValidationError("InvalidCriteriaField", critResult.Expression,
                    string.Format("'{0}' criteria field is not found!", critResult.Expression));
            if (!CanFilterField(field))
            {
                throw new ValidationError("CantFilterField", critResult.Expression,
                    string.Format("Can't filter on field '{0}'!", critResult.Expression));
            }

            return ToCriteria(field);
        }

        return result;
    }

    private bool ShouldConvertValues(BinaryCriteria criteria, [MaybeNullWhen(false)] out Field field, out object? value)
    {
        field = null;
        value = null;

        if (criteria is null ||
            criteria.Operator < CriteriaOperator.EQ ||
            criteria.Operator > CriteriaOperator.NotIn)
            return false;

        if (criteria.LeftOperand is not Criteria left)
            return false;

        field = FindField(left.Expression);
        if (field is null)
            return false;

        if (field is StringField)
            return false;

        if (criteria.RightOperand is not ValueCriteria right)
            return false;

        value = right.Value;
        return value != null;
    }

    private bool ShouldHandleLikeCriteria(BinaryCriteria? criteria)
    {
        return Dialect.IsLikeCaseSensitive &&
            criteria is not null &&
            (criteria.Operator == CriteriaOperator.Like ||
             criteria.Operator == CriteriaOperator.NotLike) &&
             criteria.RightOperand is ValueCriteria right &&
             right.Value is string &&
             criteria.LeftOperand is Criteria left &&
             left.Expression is string expression &&
             FindField(expression) is StringField;
    }

    /// <inheritdoc/>
    protected override BaseCriteria VisitBinary(BinaryCriteria criteria)
    {
        if (ShouldHandleLikeCriteria(criteria))
        {
            return new BinaryCriteria(new UpperFunctionCriteria(Visit(criteria.LeftOperand)!),
                criteria.Operator, new UpperFunctionCriteria(Visit(criteria.RightOperand)!));
        }

        if (ShouldConvertValues(criteria, out Field? field, out object? value))
        {
            var left = Visit(criteria.LeftOperand)!;
            Visit(criteria.RightOperand);

            try
            {
                var str = value as string;
                if (str == null && value is IEnumerable enumerable)
                {
                    var values = new List<object?>();
                    foreach (var v in enumerable)
                        values.Add(field.ConvertValue(v, CultureInfo.InvariantCulture));

                    return new BinaryCriteria(left, criteria.Operator, new ValueCriteria(values));
                }

                if (str == null || str.Length != 0)
                {
                    value = field.ConvertValue(value, CultureInfo.InvariantCulture);
                    return new BinaryCriteria(left, criteria.Operator, new ValueCriteria(value));
                }
            }
            catch
            {
                // swallow exceptions for backward compatibility
            }
        }

        return base.VisitBinary(criteria);
    }

}