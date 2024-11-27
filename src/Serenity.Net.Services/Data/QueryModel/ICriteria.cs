namespace Serenity;

/// <summary>
/// Shared criteria interface
/// </summary>
public interface ICriteria
{
    /// <summary>
    /// Gets a value indicating whether this criteria instance is empty.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
    /// </value>
    bool IsEmpty { get; }

    /// <summary>
    /// Converts the criteria to string representation while adding params to the target query.
    /// </summary>
    /// <param name="query">The target query to add params to.</param>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    string ToString(IQueryWithParams query);

    /// <summary>
    /// Converts the criteria to string representation into a string builder, while adding
    /// its params to the target query.
    /// </summary>
    /// <param name="sb">The string builder.</param>
    /// <param name="query">The target query to add params to.</param>
    void ToString(StringBuilder sb, IQueryWithParams query);

    /// <summary>
    /// Converts the criteria to string while ignoring its params if any.
    /// ToString() raises an exception if a criteria has params, while this not.
    /// </summary>
    /// <returns></returns>
    string ToStringIgnoreParams();
}