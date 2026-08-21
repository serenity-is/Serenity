
namespace Serenity.Data;

/// <summary>
///   Extension methods for classes implementing <see cref="ISetFieldByStatement"/>.
/// </summary>
public static class SetFieldByStatementExtensions
{
    /// <summary>
    ///   Sets a field value with a parameter.
    /// </summary>
    /// <param name="self">
    ///   The object itself.
    /// </param>
    /// <param name="field">
    ///   Field name.
    /// </param>
    /// <param name="value">
    ///   Parameter value.
    /// </param>
    /// <returns>
    ///   Object itself.
    /// </returns>
    public static T Set<T>(this T self, string field, object value) where T : ISetFieldByStatement
    {
        var param = self.AddParam(value);
        self.SetTo(field, param.Name);
        return self;
    }


}