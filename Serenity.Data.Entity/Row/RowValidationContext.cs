using Serenity.Data;
using System.Data;

namespace Serenity.Services
{
    public class RowValidationContext : IValidationContext
    {
        private Row row;

        public RowValidationContext(IDbConnection connection, Row row)
        {
            this.row = row;
            this.Connection = connection;
        }

        public object GetFieldValue(string fieldName)
        {
            var field = row.FindFieldByPropertyName(fieldName) ?? row.FindField(fieldName);
            if (ReferenceEquals(null, field))
                return null;

            return field.AsObject(row);
        }

        public IDbConnection Connection { get; private set; }
        public object Value { get; set; }
    }
}