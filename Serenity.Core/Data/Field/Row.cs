using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Serenity.Data
{
    public abstract partial class Row : IEntityWithJoins, ISupportAttached, 
        INotifyPropertyChanged, IEditableObject, IDataErrorInfo
    {
        internal RowFieldsBase fields;
        internal bool[] assignedFields;
        internal Hashtable dictionaryData;
        internal bool ignoreConstraints;
        internal object[] indexedData;
        internal bool tracking;
        internal bool unassignedReadErrors;

        protected Row(RowFieldsBase fields)
        {
            if (fields == null)
                throw new ArgumentNullException("fields");

            this.fields = fields.Init();

            TrackAssignments = TrackAssignmentsDefault;
        }

        public void CloneInto(Row clone, 
            bool cloneHandlers)
        {
            clone.ignoreConstraints = ignoreConstraints;

            foreach (var field in GetFields())
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

            clone.originalValues = originalValues;

            if (dictionaryData != null)
                clone.dictionaryData = (Hashtable)this.dictionaryData.Clone();
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
                clone.postHandler = this.postHandler;
                clone.propertyChanged = this.propertyChanged;

                if (this.validationErrors != null)
                    clone.validationErrors = new Dictionary<string, string>(this.validationErrors);
                else
                    clone.validationErrors = null;
            }
        }

        public Row CloneRow()
        {
            var clone = CreateNew();
            CloneInto(clone, true);
            return clone;
        }

        public virtual Row CreateNew()
        {
            if (fields.rowFactory == null)
                throw new NotImplementedException();

            return fields.rowFactory();
        }

        internal void FieldAssignedValue(Field field)
        {
            if (assignedFields == null)
                assignedFields = new bool[fields.Count];

            assignedFields[field.index] = true;

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

        public RowFieldsBase GetFields()
        {
            return fields;
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

        public bool IgnoreConstraints
        {
            get { return ignoreConstraints; }
            set { ignoreConstraints = value; }
        }

        public string Table
        {
            get { return fields.TableName; }
        }

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
                            previousValues = this.CloneRow();
                        tracking = value;
                    }
                    else
                    {
                        tracking = false;
                        assignedFields = null;
                    }
                }
            }
        }

        public bool UnassignedReadErrors
        {
            get { return unassignedReadErrors; }
            set { unassignedReadErrors = value; }
        }

        private Field FindFieldEnsure(string fieldName)
        {
            var field = FindField(fieldName);
            if (field == null)
                throw new ArgumentOutOfRangeException("fieldName", String.Format(
                    "{0} has no field with name '{1}'.", this.GetType().Name, fieldName));
            return field;
        }

        public object this[string fieldName]
        {
            get 
            {
                var field = FindField(fieldName);
                if (field == null)
                    return null;
                return field.AsObject(this); 
            }
            set { FindFieldEnsure(fieldName).AsObject(this, value); }
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

        public object GetDictionaryData(object key)
        {
            if (dictionaryData != null)
                return dictionaryData[key];

            return null;
        }


        internal void SetIndexedData(int index, object value)
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
                    indexedData = new object[this.FieldCount];

                indexedData[index] = value;
            }
        }

        internal object GetIndexedData(int index)
        {
            if (indexedData != null)
                return indexedData[index];

            return null;
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

        Hashtable ISupportAttached.AttachedProperties
        {
            get { return dictionaryData; }
            set { dictionaryData = value; }
        }

        IDictionary<string, Join> IEntityWithJoins.Joins
        {
            get { return fields.Joins; }
        }

        /// <summary>
        /// Only use for unit testing purposes!
        /// </summary>
        public static bool TrackAssignmentsDefault { get; set; }
    }
}