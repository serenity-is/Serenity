using Serenity.Data;
using System.Data;

namespace Serenity.Services
{
    public class RowValidationContext : IValidationContext
    {
        private IRow row;

        public RowValidationContext(IDbConnection connection, IRow row)
        {
            this.row = row;
            this.Connection = connection;
        }

        public object GetFieldValue(string fieldName)
        {
            var field = row.Fields.FindFieldByPropertyName(fieldName) ?? row.Fields.FindField(fieldName);
            if (ReferenceEquals(null, field))
                return null;

            return field.AsObject(row);
        }

        public IDbConnection Connection { get; private set; }
        public object Value { get; set; }
    }
}