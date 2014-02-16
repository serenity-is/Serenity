using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Serenity.Data
{
    public abstract partial class Row : IEntityWithJoins, ISupportAttached, 
        INotifyPropertyChanged, IEditableObject, IDataErrorInfo
    {
        internal RowFieldsBase _fields;
        internal bool[] _assignedFields;
        internal bool _tracking;
        internal bool _ignoreConstraints;
        internal Hashtable _dictionaryData;
        internal object[] _indexedData;

        protected Row(RowFieldsBase fields)
        {
            if (fields == null)
                throw new ArgumentNullException("fields");

            _fields = fields;

            if (!_fields._isLocked)
            {
                InitFields();
                _fields._isLocked = true;
            }
        }

        public void CloneInto(Row clone, 
            bool cloneHandlers)
        {
            clone._ignoreConstraints = _ignoreConstraints;

            foreach (var field in GetFields())
                field.Copy(this, clone);

            clone._tracking = _tracking;
            if (_tracking)
            {
                if (_assignedFields != null)
                {
                    clone._assignedFields = new bool[_assignedFields.Length];
                    Array.Copy(_assignedFields, clone._assignedFields, _assignedFields.Length);
                }
            }
            else
                clone._assignedFields = null;

            clone._originalValues = _originalValues;

            if (_dictionaryData != null)
                clone._dictionaryData = (Hashtable)this._dictionaryData.Clone();
            else
                clone._dictionaryData = null;

            if (_indexedData != null)
            {
                clone._indexedData = new object[_indexedData.Length];
                for (var i = 0; i < _indexedData.Length; i++)
                    clone._indexedData[i] = _indexedData[i];
            }
            else
                clone._indexedData = null;

            if (_previousValues != null)
                clone._previousValues = _previousValues.CloneRow();
            else
                clone._previousValues = null;

            if (cloneHandlers)
            {
                clone._postHandler = this._postHandler;
                clone._propertyChanged = this._propertyChanged;

                if (this._validationErrors != null)
                    clone._validationErrors = new Dictionary<string, string>(this._validationErrors);
                else
                    clone._validationErrors = null;
            }
        }

        public Row CloneRow()
        {
            var clone = CreateNew();
            CloneInto(clone, true);
            return clone;
        }

        public abstract Row CreateNew();

        internal void FieldAssignedValue(Field field)
        {
            if (_assignedFields == null)
                _assignedFields = new bool[_fields.Count];

            _assignedFields[field._index] = true;

            RemoveValidationError(field.PropertyName);

            if (_propertyChanged != null)
            {
                if (field.IndexCompare(_previousValues, this) != 0)
                {
                    RaisePropertyChanged(field);
                    field.Copy(this, _previousValues);
                }
            }
        }

        public Field FindField(string fieldName)
        {
            return _fields.FindField(fieldName);
        }

        public Field FindFieldByPropertyName(string propertyName)
        {
            return _fields.FindFieldByPropertyName(propertyName);
        }

        public RowFieldsBase GetFields()
        {
            return _fields;
        }

        public int FieldCount
        {
            get { return _fields.Count; }
        }

        public bool IsAnyFieldAssigned
        {
            get
            {
                return _tracking && _assignedFields != null;
            }
        }

        public bool IgnoreConstraints
        {
            get { return _ignoreConstraints; }
            set { _ignoreConstraints = value; }
        }

        public string Table
        {
            get { return _fields.TableName; }
        }

        public bool TrackAssignments
        {
            get
            { 
                return _tracking;
            }
            set 
            {
                if (_tracking != value)
                {
                    if (value)
                    {
                        if (_propertyChanged != null)
                            _previousValues = this.CloneRow();
                        _tracking = value;
                    }
                    else
                    {
                        _tracking = false;
                        _assignedFields = null;
                    }
                }
            }
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
                if (_dictionaryData == null)
                    return;
                _dictionaryData[key] = null;
            }
            else
            {
                if (_dictionaryData == null)
                    _dictionaryData = new Hashtable();
                _dictionaryData[key] = value;
            }
        }

        public void SetIndexedData(int index, object value)
        {
            if (value == null)
            {
                if (_indexedData == null)
                    return;

                _indexedData[index] = null;
            }
            else
            {
                if (_indexedData == null)
                    _indexedData = new object[this.FieldCount];

                _indexedData[index] = value;
            }
        }

        public object GetIndexedData(int index)
        {
            if (_indexedData != null)
                return _indexedData[index];

            return null;
        }

        public object GetDictionaryData(object key)
        {
            if (_dictionaryData != null)
                return _dictionaryData[key];

            return null;
        }

        public bool IsAssigned(Field field)
        {
            if (_assignedFields == null)
                return false;
            return _assignedFields[field._index];
        }

        public void ClearAssignment(Field field)
        {
            if (_assignedFields != null)
            {
                _assignedFields[field._index] = false;
                for (var i = 0; i < _assignedFields.Length; i++)
                    if (_assignedFields[i])
                        return;
                _assignedFields = null;
            }
        }

        public bool IsAnyFieldChanged
        {
            get
            {
                if (_originalValues == null)
                    return false;

                for (var i = 0; i < _fields.Count; i++)
                    if (_fields[i].IndexCompare(_originalValues, this) != 0)
                        return true;

                return false;
            }
        }

        Hashtable ISupportAttached.AttachedProperties
        {
            get { return _dictionaryData; }
            set { _dictionaryData = value; }
        }

        IDictionary<string, Join> IEntityWithJoins.Joins
        {
            get { return _fields.Joins; }
        }
    }
}