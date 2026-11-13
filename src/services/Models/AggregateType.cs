namespace Serenity.Services;

/// <summary>
/// The kind of a value that is calculated from the rows of a list request, see <see cref="AggregateBy"/>.
/// The values of the ones that are also in <see cref="SummaryType"/> are the same.
/// </summary>
public enum AggregateType
{
    /// <summary>
    /// The number of the rows, or of the rows whose field is not null when a field is given.
    /// </summary>
    Count = 0,
    /// <summary>
    /// The sum of the values of the field.
    /// </summary>
    Sum = 1,
    /// <summary>
    /// The average of the values of the field, without the null ones.
    /// </summary>
    Avg = 2,
    /// <summary>
    /// The smallest value of the field.
    /// </summary>
    Min = 3,
    /// <summary>
    /// The largest value of the field.
    /// </summary>
    Max = 4,
    /// <summary>
    /// The number of the different values of the field, without null.
    /// </summary>
    DistinctCount = 5
}
