using Serenity.Data;
using System.Data;

namespace Serenity.Services
{
    public class RowValidationContext : IValidationContext
    {
        private Row row;
        private IDbConnection connection;

        public RowValidationContext(IDbConnection connection, Row row)
        {
            this.row = row;
            this.connection = connection;
        }

        public object GetFieldValue(string fieldName)
        {
            var field = row.FindFieldByPropertyName(fieldName) ?? row.FindField(fieldName);
            if (field == null)
                return null;

            return field.AsObject(row);
        }

        public IDbConnection Connection { get; private set; }
        public object Value { get; set; }
    }
}