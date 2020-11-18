using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Serenity.Data
{
    [JsonConverter(typeof(JsonRowConverter))]
    public abstract partial class Row<TFields> : IRow, IRow<TFields>, INotifyPropertyChanged, IEditableObject
        where TFields: RowFieldsBase
    {
        protected readonly TFields fields;
        internal bool[] assignedFields;
        internal Hashtable dictionaryData;
        internal object[] indexedData;
        internal bool tracking;
        internal bool trackWithChecks;

        protected Row(TFields fields = null)
        {
            if (fields == null)
            {
                var fieldsProvider = RowFieldsProvider.Current;
                fields = (TFields)fieldsProvider.Resolve(typeof(TFields));
            }

            if (!fields.isInitialized)
                throw new ArgumentOutOfRangeException("fields", $"{GetType().FullName} constructor is called " +
                    $"with a fields object that is not initialized. Please call .Init() method on it before using!");

            this.fields = fields;

            fields.RowCreated(this);

            TrackAssignments = true;
        }

        public TFields Fields => fields;
        RowFieldsBase IRow.Fields => fields;

        public void CloneInto(Row<TFields> clone, 
            bool cloneHandlers)
        {
            clone.IgnoreConstraints = IgnoreConstraints;

            foreach (var field in fields)
                field.Copy(this, clone);

            clone.tracking = tracking;
            if (tracking)
            {
                if (assignedFields != null)
                {
                    clone.assignedFields = new bool[assignedFields.Length];
                    Array.Copy(assignedFields, clone.assignedFields, assignedFields.Length);
                }
            }
            else
                clone.assignedFields = null;

            clone.trackWithChecks = trackWithChecks;

            clone.originalValues = originalValues;

            if (dictionaryData != null)
                clone.dictionaryData = (Hashtable)dictionaryData.Clone();
            else
                clone.dictionaryData = null;

            if (indexedData != null)
            {
                clone.indexedData = new object[indexedData.Length];
                for (var i = 0; i < indexedData.Length; i++)
                    clone.indexedData[i] = indexedData[i];
            }
            else
                clone.indexedData = null;

            if (previousValues != null)
                clone.previousValues = previousValues.CloneRow();
            else
                clone.previousValues = null;

            if (cloneHandlers)
            {
                clone.postHandler = postHandler;
                clone.propertyChanged = propertyChanged;

                if (validationErrors != null)
                    clone.validationErrors = new Dictionary<string, string>(validationErrors);
                else
                    clone.validationErrors = null;
            }
        }

        public virtual Row<TFields> CloneRow()
        {
            var clone = CreateNew();
            CloneInto(clone, true);
            return clone;
        }

        public virtual Row<TFields> CreateNew()
        {
            if (fields.rowFactory == null)
                throw new NotImplementedException();

            return (Row<TFields>)fields.rowFactory();
        }

        IRow<TFields> IRow<TFields>.CreateNew()
        {
            return CreateNew();
        }

        IRow<TFields> IRow<TFields>.CloneRow()
        {
            return CloneRow();
        }

        IRow IRow.CloneRow()
        {
            return CloneRow();
        }

        IRow IRow.CreateNew()
        {
            return CreateNew();
        }

        void IRow.FieldAssignedValue(Field field)
        {
            if (!tracking)
                return;

            if (assignedFields == null)
                assignedFields = new bool[fields.Count];

            assignedFields[field.index] = true;

            if (validationErrors != null)
                RemoveValidationError(field.PropertyName ?? field.Name);

            if (propertyChanged != null)
            {
                if (field.IndexCompare(previousValues, this) != 0)
                {
                    RaisePropertyChanged(field);
                    field.Copy(this, previousValues);
                }
            }
        }

        public Field FindField(string fieldName)
        {
            return fields.FindField(fieldName);
        }

        public Field FindFieldByPropertyName(string propertyName)
        {
            return fields.FindFieldByPropertyName(propertyName);
        }

        public int FieldCount
        {
            get { return fields.Count; }
        }

        public bool IsAnyFieldAssigned
        {
            get
            {
                return tracking && assignedFields != null;
            }
        }

        public bool IgnoreConstraints { get; set; }

        public string Table => fields.TableName;

        public bool TrackAssignments
        {
            get
            { 
                return tracking;
            }
            set 
            {
                if (tracking != value)
                {
                    if (value)
                    {
                        if (propertyChanged != null)
                            previousValues = CloneRow();

                        tracking = value;
                    }
                    else
                    {
                        tracking = false;
                        trackWithChecks = false;
                        assignedFields = null;
                    }
                }
            }
        }

        public bool TrackWithChecks
        {
            get 
            {
                return tracking && trackWithChecks;
            }
            set
            {
                if (value != TrackWithChecks)
                {
                    if (value && !tracking)
                        TrackAssignments = true;

                    trackWithChecks = value;
                }
            }
        }

        private Field FindFieldEnsure(string fieldName)
        {
            var field = FindField(fieldName);
            if (field is null)
                throw new ArgumentOutOfRangeException("fieldName", string.Format(
                    "{0} has no field with name '{1}'.", GetType().Name, fieldName));
            return field;
        }

        public object this[string fieldName]
        {
            get 
            {
                var field = FindFieldByPropertyName(fieldName) ??
                    FindField(fieldName);

                if (field is null)
                {
                    if (dictionaryData != null)
                        return dictionaryData[fieldName];

                    return null;
                }

                return field.AsObject(this); 
            }
            set
            {
                (FindFieldByPropertyName(fieldName) ?? 
                    FindFieldEnsure(fieldName)).AsObject(this, value);
            }
        }

        public void SetDictionaryData(object key, object value)
        {
            if (value == null)
            {
                if (dictionaryData == null)
                    return;
                dictionaryData[key] = null;
            }
            else
            {
                if (dictionaryData == null)
                    dictionaryData = new Hashtable();
                dictionaryData[key] = value;
            }
        }

        public IEnumerable GetDictionaryDataKeys()
        {
            if (dictionaryData == null)
                return Array.Empty<object>();

            return dictionaryData.Keys;
        }

        public object GetDictionaryData(object key)
        {
            if (dictionaryData != null)
                return dictionaryData[key];

            return null;
        }

        object IRow.GetIndexedData(int index)
        {
            if (indexedData != null)
                return indexedData[index];

            return null;
        }

        void IRow.SetIndexedData(int index, object value)
        {
            if (value == null)
            {
                if (indexedData == null)
                    return;

                indexedData[index] = null;
            }
            else
            {
                if (indexedData == null)
                    indexedData = new object[FieldCount];

                indexedData[index] = value;
            }
        }

        public bool IsAssigned(Field field)
        {
            if (assignedFields == null)
                return false;

            return assignedFields[field.index];
        }

        public void ClearAssignment(Field field)
        {
            if (assignedFields == null)
                return;

            assignedFields[field.index] = false;

            for (var i = 0; i < assignedFields.Length; i++)
                if (assignedFields[i])
                    return;

            assignedFields = null;
        }

        public bool IsAnyFieldChanged
        {
            get
            {
                if (originalValues == null)
                    return false;

                for (var i = 0; i < fields.Count; i++)
                    if (fields[i].IndexCompare(originalValues, this) != 0)
                        return true;

                return false;
            }
        }

        IDictionary<string, Join> IHaveJoins.Joins
        {
            get { return fields.Joins; }
        }

        public Field IdField => Fields.IdField;

        public Field NameField => Fields.NameField;
    }
}