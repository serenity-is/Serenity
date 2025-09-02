namespace Serenity.Data;

/// <summary>
/// Select level enumeration
/// </summary>
public enum SelectLevel
{
    /// <summary>
    /// Auto is equivalent to List level for table fields and Details level for view fields.
    /// </summary>
    Auto = 0,
    /// <summary>
    /// Always select this field, even if it is in ExcludeColumns list
    /// </summary>
    Always = 1,
    /// <summary>
    /// Select this field in ColumnSelection.Lookup, ColumnSelection.List or ColumnSelection.Details modes,
    /// or if it's specified in IncludeColumns list (if not also in 
    /// ExcludeColumns list)
    /// </summary>
    Lookup = 2,
    /// <summary>
    /// Select this field in ColumnSelection.List or ColumnSelection.Details modes,
    /// or if it's specified in IncludeColumns list (if not also in 
    /// ExcludeColumns list)
    /// </summary>
    List = 3,
    /// <summary>
    /// Select this field in ColumnSelection.Details mode or if it's specified in 
    /// IncludeColumns list (if not also in ExcludeColumns list)
    /// </summary>
    Details = 4,
    /// <summary>
    /// Don't select this field in any mode. Select it if specified in 
    /// IncludeColumns list (if not also in ExcludeColumns list)
    /// </summary>
    Explicit = 5,
    /// <summary>
    /// Never select this column. Use this for sensitive fields like password hash.
    /// </summary>
    Never = 6
}