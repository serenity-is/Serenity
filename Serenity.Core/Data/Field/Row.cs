using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;

namespace Serenity.Data
{
    public interface ISupportAttached
    {
        Hashtable AttachedProperties { get; set; }
    }

    public abstract class Row : INotifyPropertyChanged, IEditableObject, IDataErrorInfo, ISupportAttached
    {
        internal RowFieldsBase _fields;
        internal int _insidePostHandler;
        internal bool[] _assignedFields;
        internal bool _tracking;
        internal bool _ignoreConstraints;
        internal Row _originalValues;
        internal Row _previousValues;
        internal Hashtable _dictionaryData;
        internal PropertyChangedEventHandler _propertyChanged;
        internal Action<Row> _postHandler;
        private Dictionary<String, String> _validationErrors;
        //internal static object _initializationLock = new object();

        /*
            #if SILVERLIGHT
            #endif

            }
        }*/

        protected Row(RowFieldsBase fields)
        {
            if (fields == null)
                throw new ArgumentNullException("fields");

            _fields = fields;

            if (!_fields._isLocked)
            {
                foreach (var fieldInfo in _fields.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (fieldInfo.FieldType.IsSubclassOf(typeof(Field)))
                    {
                        var field = fieldInfo.GetValue(_fields) as Field;
                        if (field != null)
                            field._propertyName = fieldInfo.Name;
                    }
                }

                var byPropertyName = new Dictionary<string, Field>();

                foreach (var field in _fields)
                {
                    field._propertyName = field._propertyName ?? field._name;
                    byPropertyName[field._propertyName ?? field._name] = field;
                }

                _fields._byPropertyName = byPropertyName;

                PropertyDescriptor[] properties = new PropertyDescriptor[_fields.Count];
                for (int i = 0; i < _fields.Count; i++)
                {
                    Field field = _fields[i];
                    field._rowType = this.GetType();
                    properties[i] = new FieldDescriptor(field);
                }

                _fields._propertyDescriptors = new PropertyDescriptorCollection(properties);

                var schemaAttr = this.GetType().GetCustomAttribute<SchemaAttribute>();
                if (schemaAttr != null)
                    _fields._schema = schemaAttr.Schema;
                else
                    _fields._schema = "Default";

                _fields._isLocked = true;
            }


        /* _getValue ve _setValue bu şekilde dinamik oluşturulabilir ancak Silverlight gibi platformlarda private member lara erişmek
         * mümkün değil, o nedenle iptal edildi, delegate olarak verilmeleri zorunlu malesef

            if (!_fields._isLocked)
            {
                lock (_initializationLock)
                {
                    if (_fields._isLocked)
                        return;

                    Dictionary<string, FieldInfo> valueInfo = new Dictionary<string, FieldInfo>();
                    var values = this.GetType().GetMembers(BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (var value in values)
                    {
                        var fi = value as FieldInfo;
                        if (fi != null)
                            valueInfo[fi.Name] = fi;
                    }

                    var members = _fields.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var member in members)
                    {
                        var fi = member as FieldInfo;
                        if (fi != null && 
                            fi.FieldType.IsSubclassOf(typeof(Field)))
                        {
                            Field f = (Field)fi.GetValue(_fields);
                            if (f != null)
                            {
                                f._fieldProperty = fi;

                                FieldInfo valueProperty;
                                if (valueInfo.TryGetValue("_" + fi.Name, out valueProperty))
                                    f._valueProperty = valueProperty;
                            }
                        }
                    }

                    foreach (var field in _fields)
                        field.OnRowInitialization();

                    _fields._isLocked = true;
                }
            }*/
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
                        //EndEdit();
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

        internal void RaisePropertyChanged(Field field)
        {
            if (_fields._propertyChangedEventArgs == null)
            {
                var args = new PropertyChangedEventArgs[_fields.Count + 1];
                for (var i = 0; i < _fields.Count; i++)
                {
                    var f = _fields[i];
                    args[i] = new PropertyChangedEventArgs(f._propertyName ?? f.Name);
                }
                args[_fields.Count] = new PropertyChangedEventArgs("__ROW__");
                _fields._propertyChangedEventArgs = args;
            }

            if (field == null)
                _propertyChanged(this, _fields._propertyChangedEventArgs[_fields.Count]);
            else
                _propertyChanged(this, _fields._propertyChangedEventArgs[field._index]);
        }

        public Action<Row> PostHandler
        {
            get { return _postHandler; }
            set { _postHandler = value; }
        }

        public bool IsFieldChanged(Field field)
        {
            return (_originalValues != null &&
                    field.IndexCompare(_originalValues, this) != 0);
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add 
            {
                _propertyChanged += value;
                if (_previousValues == null)
                    _previousValues = this.CloneRow();
            }
            remove 
            {
                _propertyChanged -= value; 
            }
        }

        public void BeginEdit()
        {
            TrackAssignments = true;

            if (_originalValues == null)
                _originalValues = this.CloneRow();
        }

        public void CancelEdit()
        {
            if (_originalValues != null)
            {
                var original = _originalValues;

                _originalValues = null;

                for (int i = 0; i < _fields.Count; i++)
                    _fields[i].CopyNoAssignment(original, this);

                _assignedFields = original._assignedFields;

                ClearValidationErrors();
            }
        }

        public void EndEdit()
        {
            if (_postHandler != null &&
                _originalValues != null)
            {
                if (_insidePostHandler > 0)
                    return; // exception daha iyi olabilir mi?

                _insidePostHandler++;
                try
                {
                    ClearValidationErrors();
                    _postHandler(this);
                    if (HasErrors)
                        throw new Exception("Lütfen satırdaki işaretli alanları düzeltiniz.");
                    _originalValues = null;
                    //_assignedFields = null;
                }
                finally
                {
                    _insidePostHandler--;
                }

                if (PostEnded != null)
                    PostEnded(this, new EventArgs());
            }
            else
            {
                //_assignedFields = null;
                _originalValues = null;
                ClearValidationErrors();
            }
        }

        public bool IsEditing
        {
            get { return _originalValues != null; }
        }

        public Row OriginalValues
        {
            get { return _originalValues ?? this; }
        }

        public Row PreviousValues
        {
            get { return _previousValues ?? this; }
        }

        public bool HasPostHandler
        {
            get { return _postHandler != null; }
        }

        public EventHandler PostEnded;

        public void AddValidationError(string propertyName, string error)
        {
            if (_validationErrors == null)
                _validationErrors = new Dictionary<string, string>();

            _validationErrors[propertyName ?? String.Empty] = error;
        }

        public void ClearValidationErrors()
        {
            if (_validationErrors != null &&
                _validationErrors.Count > 0)
            {
                _validationErrors.Clear();
            }
        }

        public void RemoveValidationError(string propertyName)
        {
            if (_validationErrors != null)
                _validationErrors.Remove(propertyName ?? String.Empty);
        }

        public IDictionary<string, string> ValidationErrors
        {
            get { return _validationErrors; }
        }

        public bool HasErrors
        {
            get
            {
                return
                    _validationErrors != null &&
                    _validationErrors.Count > 0;
            }
        }

        string IDataErrorInfo.Error
        {
            get
            {
                string error;
                if (_validationErrors != null &&
                    _validationErrors.TryGetValue(String.Empty, out error))
                    return error;
                return String.Empty;
            }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                string error;
                if (_validationErrors != null &&
                    _validationErrors.TryGetValue(columnName ?? String.Empty, out error))
                    return error;
                return String.Empty;
            }
        }

        Hashtable ISupportAttached.AttachedProperties
        {
            get { return _dictionaryData; }
            set { _dictionaryData = value; }
        }

        public PropertyDescriptorCollection GetPropertyDescriptors()
        {
            return _fields._propertyDescriptors;
        }
    }
}