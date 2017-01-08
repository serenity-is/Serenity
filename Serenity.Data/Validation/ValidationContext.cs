using System.Data;

namespace Serenity
{
    public interface IValidationContext
    {
        object Value { get; }
        object GetFieldValue(string fieldName);
        IDbConnection Connection { get; }
    }
}