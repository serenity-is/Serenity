
namespace Serenity.Data;

/// <summary>
///   Extension methods for classes implementing IDbSet interface</summary>
public static class SetFieldByStatementExtensions
{
    /// <summary>
    ///   Sets a field value with a parameter.</summary>
    /// <param field="field">
    ///   Field name.</param>
    /// <param field="param">
    ///   Parameter name</param>
    /// <param field="value">
    ///   Parameter value</param>
    /// <returns>
    ///   Object itself.</returns>
    public static T Set<T>(this T self, string field, object value) where T : ISetFieldByStatement
    {
        var param = self.AddParam(value);
        self.SetTo(field, param.Name);
        return self;
    }


}