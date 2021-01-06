using Serenity.Data;
using System.Data;

namespace Serenity.Services
{
    public class RowValidationContext : IValidationContext
    {
        private readonly IRow row;

        public RowValidationContext(IDbConnection connection, IRow row, ITextLocalizer localizer)
        {
            this.row = row;
            Connection = connection;
            Localizer = localizer;
        }

        public object GetFieldValue(string fieldName)
        {
            var field = row.Fields.FindFieldByPropertyName(fieldName) ?? row.Fields.FindField(fieldName);
            if (field is null)
                return null;

            return field.AsObject(row);
        }

        public IDbConnection Connection { get; private set; }
        public object Value { get; set; }
        public ITextLocalizer Localizer { get; private set; }
    }
}