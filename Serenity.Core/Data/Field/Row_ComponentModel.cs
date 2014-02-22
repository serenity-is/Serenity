using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Serenity.Data
{
    public abstract partial class Row
    {
        internal int _insidePostHandler;
        internal Row _originalValues;
        internal Row _previousValues;
        internal PropertyChangedEventHandler _propertyChanged;
        internal Action<Row> _postHandler;
        private Dictionary<String, String> _validationErrors;

        internal void RaisePropertyChanged(Field field)
        {
            if (_fields.propertyChangedEventArgs == null)
            {
                var args = new PropertyChangedEventArgs[_fields.Count + 1];
                for (var i = 0; i < _fields.Count; i++)
                {
                    var f = _fields[i];
                    args[i] = new PropertyChangedEventArgs(f._propertyName ?? f.Name);
                }
                args[_fields.Count] = new PropertyChangedEventArgs("__ROW__");
                _fields.propertyChangedEventArgs = args;
            }

            if (field == null)
                _propertyChanged(this, _fields.propertyChangedEventArgs[_fields.Count]);
            else
                _propertyChanged(this, _fields.propertyChangedEventArgs[field._index]);
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

        public PropertyDescriptorCollection GetPropertyDescriptors()
        {
            return _fields.propertyDescriptors;
        }
    }
}