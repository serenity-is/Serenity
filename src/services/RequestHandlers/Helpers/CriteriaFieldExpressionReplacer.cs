using System.Collections;

namespace Serenity.Data;

/// <summary>
/// Converts field names in a criteria to their 
/// corresponding SQL field expressions.
/// </summary>
/// <remarks>
/// Creates an instance of the class
/// </remarks>
/// <param name="row">The row instance</param>
/// <param name="permissions">Permission service</param>
/// <param name="lookupAccessMode">Use lookup access mode.
/// In the lookup access mode only the lookup fields can be
/// used in the filter. Default is false.</param>
/// <param name="dialect">Optional dialect</param>
/// <param name="toCriteria">Optional field to criteria converter</param>
/// <exception cref="ArgumentNullException">row or permissions is null</exception>
public class CriteriaFieldExpressionReplacer(IRow row, IPermissionService permissions,
    bool lookupAccessMode = false, ISqlDialect dialect = null, Func<IField, BaseCriteria> toCriteria = null) : SafeCriteriaValidator
{
    private readonly IPermissionService permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
    private readonly bool lookupAccessMode = lookupAccessMode;

    /// <summary>
    /// The row instance
    /// </summary>
    protected IRow Row { get; private set; } = row ?? throw new ArgumentNullException(nameof(row));

    /// <summary>
    /// The dialect passed in or default dialect
    /// </summary>
    protected ISqlDialect Dialect { get; } = dialect ?? SqlSettings.DefaultDialect;

    /// <summary>
    /// Visits the criteria for conversion and returns
    /// a processed criteria containing replaced field
    /// expressions.
    /// </summary>
    /// <param name="criteria">The criteria</param>
    public BaseCriteria Process(BaseCriteria criteria)
    {
        return Visit(criteria);
    }

    /// <summary>
    /// Virtual method to check if a Field can be filtered.
    /// </summary>
    /// <param name="field">Field instance</param>
    protected virtual bool CanFilterField(Field field)
    {
        if (field.Flags.HasFlag(FieldFlags.DenyFiltering) ||
            field.Flags.HasFlag(FieldFlags.NotMapped))
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
    protected virtual Field FindField(string expression)
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
        var result = base.VisitCriteria(criteria);

        criteria = result as Criteria;

        if (criteria is not null)
        {
            var field = FindField(criteria.Expression) ?? throw new ValidationError("InvalidCriteriaField", criteria.Expression,
                    string.Format("'{0}' criteria field is not found!", criteria.Expression));
            if (!CanFilterField(field))
            {
                throw new ValidationError("CantFilterField", criteria.Expression,
                    string.Format("Can't filter on field '{0}'!", criteria.Expression));
            }

            return ToCriteria(field);
        }

        return result;
    }

    private bool ShouldConvertValues(BinaryCriteria criteria, out Field field, out object value)
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

    private bool ShouldHandleLikeCriteria(BinaryCriteria criteria, out Field field)
    {
        field = null;
        return Dialect.IsLikeCaseSensitive &&
            criteria is not null &&
            (criteria.Operator == CriteriaOperator.Like ||
             criteria.Operator == CriteriaOperator.NotLike) &&
             criteria.RightOperand is ValueCriteria right &&
             right.Value is string &&
             criteria.LeftOperand is Criteria left &&
             ((field = FindField(left.Expression)) is StringField);
    }

    /// <inheritdoc/>
    protected override BaseCriteria VisitBinary(BinaryCriteria criteria)
    {
        if (ShouldHandleLikeCriteria(criteria, out Field fieldToUpper))
        {
            return new BinaryCriteria(new UpperFunctionCriteria(ToCriteria(fieldToUpper)), 
                criteria.Operator, new UpperFunctionCriteria(criteria.RightOperand));
        }

        if (ShouldConvertValues(criteria, out Field field, out object value))
            try
            {
                var str = value as string;
                if (str == null && value is IEnumerable)
                {
                    var values = new List<object>();
                    foreach (var v in value as IEnumerable)
                        values.Add(field.ConvertValue(v, CultureInfo.InvariantCulture));

                    return new BinaryCriteria(ToCriteria(field), criteria.Operator, new ValueCriteria(values));
                }

                if (str == null || str.Length != 0)
                {
                    value = field.ConvertValue(value, CultureInfo.InvariantCulture);
                    return new BinaryCriteria(ToCriteria(field), criteria.Operator, new ValueCriteria(value));
                }
            }
            catch
            {
                // swallow exceptions for backward compatibility
            }

        return base.VisitBinary(criteria);
    }

}