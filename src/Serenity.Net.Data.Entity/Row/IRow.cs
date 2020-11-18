using System.Collections;

namespace Serenity.Data
{
    public interface IRow : IEntityWithJoins
    {
        IRow CreateNew();
        IRow CloneRow();
        RowFieldsBase Fields { get; }
        void FieldAssignedValue(Field field);
        object GetDictionaryData(object key);
        IEnumerable GetDictionaryDataKeys();
        void SetDictionaryData(object key, object value);
        object GetIndexedData(int index);
        void SetIndexedData(int index, object value);
        bool TrackAssignments { get; set; }
        bool TrackWithChecks { get; set; }
        bool IsAnyFieldAssigned { get; }
        bool IsAnyFieldChanged { get; }
        bool IsAssigned(Field field);
        void ClearAssignment(Field field);
        Field IdField { get; }
        Field NameField { get; }
        object this[string fieldName] { get; set; }
    }

    public interface IRow<TFields> : IRow
    {
        public new IRow<TFields> CreateNew();
        public new IRow<TFields> CloneRow();
        public new TFields Fields { get; }
    }
}