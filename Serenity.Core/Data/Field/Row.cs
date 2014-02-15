using Serenity.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Emit;

namespace Serenity.Data
{
    public abstract class Row : INotifyPropertyChanged, IEditableObject, IDataErrorInfo, ISupportAttached, IEntity
    {
        internal RowFieldsBase _fields;
        internal int _insidePostHandler;
        internal bool[] _assignedFields;
        internal bool _tracking;
        internal bool _ignoreConstraints;
        internal Row _originalValues;
        internal Row _previousValues;
        internal Hashtable _dictionaryData;
        internal object[] _indexedData;
        internal PropertyChangedEventHandler _propertyChanged;
        internal Action<Row> _postHandler;
        private Dictionary<String, String> _validationErrors;

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

        private void InitFields()
        {
            InitFields_PropertyNames();
            InitFields_ByPropertyName();
            InitFields_PropertyDescriptors();
            InitFields_Schema();
            InitFields_InferTextualFields();
        }

        private void InitFields_GetMyFieldsAndProperties(
            out Dictionary<string, FieldInfo> myFields,
            out Dictionary<string, PropertyInfo> myProperties)
        {
            myFields = new Dictionary<string, FieldInfo>(StringComparer.OrdinalIgnoreCase);
            myProperties = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);

            var members = this.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var member in members)
            {
                var fi = member as FieldInfo;
                if (fi != null)
                    myFields[fi.Name] = fi;
                else
                {
                    var pi = member as PropertyInfo;
                    if (pi != null)
                        myProperties[pi.Name] = pi;
                }
            }
        }

        private static Delegate CreateGetMethod(FieldInfo fieldInfo)
        {
            Type[] arguments = new Type[1];
            arguments[0] = typeof(Row);

            var getter = new DynamicMethod(String.Concat("_Get", fieldInfo.Name, "_"), 
                fieldInfo.FieldType, arguments, fieldInfo.DeclaringType);

            ILGenerator generator = getter.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Castclass, fieldInfo.DeclaringType);
            generator.Emit(OpCodes.Ldfld, fieldInfo);
            generator.Emit(OpCodes.Ret);
            return getter.CreateDelegate(typeof(Func<,>).MakeGenericType(typeof(Row), fieldInfo.FieldType));
        }

        private static Delegate CreateSetMethod(FieldInfo fieldInfo)
        {
            Type[] arguments = new Type[2];
            arguments[0] = typeof(Row);
            arguments[1] = fieldInfo.FieldType;

            var getter = new DynamicMethod(String.Concat("_Set", fieldInfo.Name, "_"),
                null, arguments, fieldInfo.DeclaringType);

            ILGenerator generator = getter.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Castclass, fieldInfo.DeclaringType);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Stfld, fieldInfo);
            generator.Emit(OpCodes.Ret);
            return getter.CreateDelegate(typeof(Action<,>).MakeGenericType(typeof(Row), fieldInfo.FieldType));
        }

        private void InitFields_PropertyNames()
        {
            Dictionary<string, FieldInfo> myFields;
            Dictionary<string, PropertyInfo> myProperties;
            InitFields_GetMyFieldsAndProperties(out myFields, out myProperties);
                    
            foreach (var fieldInfo in _fields.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                if (fieldInfo.FieldType.IsSubclassOf(typeof(Field)))
                {
                    var field = (Field)fieldInfo.GetValue(_fields);

                    PropertyInfo property;
                    if (!myProperties.TryGetValue(fieldInfo.Name, out property))
                        property = null;

                    ColumnAttribute column = null;
                    DisplayNameAttribute display = null;
                    SizeAttribute size = null;
                    ExpressionAttribute expression = null;
                    ScaleAttribute scale = null;

                    FieldFlags addFlags = (FieldFlags)0;
                    FieldFlags removeFlags = (FieldFlags)0;

                    if (property != null)
                    {
                        column = property.GetCustomAttribute<ColumnAttribute>(false);
                        display = property.GetCustomAttribute<DisplayNameAttribute>(false);
                        size = property.GetCustomAttribute<SizeAttribute>(false);
                        expression = property.GetCustomAttribute<ExpressionAttribute>(false);
                        scale = property.GetCustomAttribute<ScaleAttribute>(false);

                        var insertable = property.GetCustomAttribute<InsertableAttribute>(false);
                        var updatable = property.GetCustomAttribute<UpdatableAttribute>(false);

                        if (insertable != null && !insertable.Value)
                            removeFlags |= FieldFlags.Insertable;

                        if (updatable != null && !updatable.Value)
                            removeFlags |= FieldFlags.Updatable;

                        if (property.GetCustomAttribute<NotNullAttribute>(false) != null)
                            addFlags |= FieldFlags.NotNull;

                        if (property.GetCustomAttribute<PrimaryKeyAttribute>(false) != null)
                            addFlags |= FieldFlags.PrimaryKey;

                        if (property.GetCustomAttribute<AutoIncrementAttribute>(false) != null)
                            addFlags |= FieldFlags.AutoIncrement;

                        var add = property.GetCustomAttribute<AddFlagsAttribute>(false);
                        if (add != null)
                            addFlags |= add.Value;
                    }

                    if (field == null)
                    {
                        if (property == null)
                        {
                            throw new InvalidProgramException(String.Format(
                                "Field {0} in type {1} is null and has no corresponding property in entity!",
                                    fieldInfo.Name, _fields.GetType().Name));
                        }

                        object[] prm = new object[7];
                        prm[0] = this._fields; // owner
                        prm[1] = column == null ? property.Name : (column.Name.TrimToNull() ?? property.Name);
                        prm[2] = display != null ? new LocalText(display.DisplayName) : null;
                        prm[3] = size != null ? size.Value : 0;
                        prm[4] = (FieldFlags)((FieldFlags.Default | addFlags) - removeFlags);
                        prm[5] = null;
                        prm[6] = null;

                        FieldInfo storage;
                        if (myFields.TryGetValue("_" + property.Name, out storage) ||
                            myFields.TryGetValue("m_" + property.Name, out storage) ||
                            myFields.TryGetValue(property.Name.Substring(1), out storage))
                        {
                            prm[5] = CreateGetMethod(storage);
                            prm[6] = CreateSetMethod(storage);
                        }

                        field = (Field)Activator.CreateInstance(fieldInfo.FieldType, prm);
                        fieldInfo.SetValue(_fields, field);

                    }
                    else 
                    {
                        if (size != null)
                            field.Size = size.Value;

                        if (display != null)
                            field.Caption = new LocalText(display.DisplayName);

                        if ((int)addFlags != 0 || (int)removeFlags != 0)
                            field.Flags = (FieldFlags)((field.Flags | addFlags) - removeFlags);
                    }

                    if (expression != null)
                        field.Expression = expression.Value;

                    if (scale != null)
                        field.Scale = scale.Value;

                    if (property != null)
                        field.PropertyName = property.Name;
                }
            }
        }

        private void InitFields_Schema()
        {
            var schemaAttr = this.GetType().GetCustomAttribute<SchemaAttribute>();
            if (schemaAttr != null)
                _fields._schema = schemaAttr.Schema;
            else
                _fields._schema = "Default";
        }

        private void InitFields_ByPropertyName()
        {
            var byPropertyName = new Dictionary<string, Field>();

            foreach (var field in _fields)
            {
                field._propertyName = field._propertyName ?? field._name;
                byPropertyName[field._propertyName ?? field._name] = field;
            }

            _fields._byPropertyName = byPropertyName;
        }

        private void InitFields_PropertyDescriptors()
        {
            PropertyDescriptor[] properties = new PropertyDescriptor[_fields.Count];
            for (int i = 0; i < _fields.Count; i++)
            {
                Field field = _fields[i];
                field._rowType = this.GetType();
                properties[i] = new FieldDescriptor(field);
            }

            _fields._propertyDescriptors = new PropertyDescriptorCollection(properties);
        }

        private void InitFields_InferTextualFields()
        {
            foreach (var field in _fields)
            {
                if (!field.ForeignTable.IsEmptyOrNull() &&
                    field.TextualField == null)
                {
                    foreach (var join in _fields._joins.Values)
                    {
                        if (String.Compare(field.ForeignTable, join.Table) == 0 &&
                            join is LeftJoin &&
                            !Object.ReferenceEquals(null, join.OnCriteria) &&
                            join.OnCriteria.ToStringIgnoreParams().IndexOf(field.Expression, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            foreach (var f in _fields)
                                if (String.Compare(f.JoinAlias, join.Name, StringComparison.OrdinalIgnoreCase) == 0 &&
                                    f is StringField)
                                {
                                    field.TextualField = f.Name;
                                    break;
                                }
                        }

                        if (field.TextualField != null)
                            break;
                    }
                }
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