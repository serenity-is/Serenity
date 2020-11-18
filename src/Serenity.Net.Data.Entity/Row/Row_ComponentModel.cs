using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Serenity.Data
{
    public abstract partial class Row<TFields>
    {
        internal int insidePostHandler;
        internal Row<TFields> originalValues;
        internal Row<TFields> previousValues;
        internal PropertyChangedEventHandler propertyChanged;
        internal Action<Row<TFields>> postHandler;
        private Dictionary<string, string> validationErrors;

        internal void RaisePropertyChanged(Field field)
        {
            if (fields.propertyChangedEventArgs == null)
            {
                var args = new PropertyChangedEventArgs[fields.Count + 1];
                for (var i = 0; i < fields.Count; i++)
                {
                    var f = fields[i];
                    args[i] = new PropertyChangedEventArgs(f.propertyName ?? f.Name);
                }
                args[fields.Count] = new PropertyChangedEventArgs("__ROW__");
                fields.propertyChangedEventArgs = args;
            }

            if (field is null)
                propertyChanged(this, fields.propertyChangedEventArgs[fields.Count]);
            else
                propertyChanged(this, fields.propertyChangedEventArgs[field.Index]);
        }

        public Action<Row<TFields>> PostHandler
        {
            get { return postHandler; }
            set { postHandler = value; }
        }

        public bool IsFieldChanged(Field field)
        {
            return (originalValues != null &&
                    field.IndexCompare(originalValues, this) != 0);
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                propertyChanged += value;
                if (previousValues == null)
                    previousValues = CloneRow();
            }
            remove
            {
                propertyChanged -= value;
            }
        }

        public void BeginEdit()
        {
            TrackAssignments = true;

            if (originalValues == null)
                originalValues = CloneRow();
        }

        public void CancelEdit()
        {
            if (originalValues != null)
            {
                var original = originalValues;

                originalValues = null;

                for (int i = 0; i < fields.Count; i++)
                    fields[i].CopyNoAssignment(original, this);

                assignedFields = original.assignedFields;

                ClearValidationErrors();
            }
        }

        public void EndEdit()
        {
            if (postHandler != null &&
                originalValues != null)
            {
                if (insidePostHandler > 0)
                    return; // exception daha iyi olabilir mi?

                insidePostHandler++;
                try
                {
                    ClearValidationErrors();
                    postHandler(this);
                    if (HasErrors)
                        throw new Exception("Lütfen satırdaki işaretli alanları düzeltiniz.");
                    originalValues = null;
                }
                finally
                {
                    insidePostHandler--;
                }

                PostEnded?.Invoke(this, new EventArgs());
            }
            else
            {
                originalValues = null;
                ClearValidationErrors();
            }
        }

        public bool IsEditing
        {
            get { return originalValues != null; }
        }

        public Row<TFields> OriginalValues
        {
            get { return originalValues ?? this; }
        }

        public Row<TFields> PreviousValues
        {
            get { return previousValues ?? this; }
        }

        public bool HasPostHandler
        {
            get { return postHandler != null; }
        }

        public EventHandler PostEnded;

        public void AddValidationError(string propertyName, string error)
        {
            if (validationErrors == null)
                validationErrors = new Dictionary<string, string>();

            validationErrors[propertyName ?? string.Empty] = error;
        }

        public void ClearValidationErrors()
        {
            if (validationErrors != null &&
                validationErrors.Count > 0)
            {
                validationErrors.Clear();
            }
        }

        public void RemoveValidationError(string propertyName)
        {
            if (validationErrors != null)
                validationErrors.Remove(propertyName ?? string.Empty);
        }

        public IDictionary<string, string> ValidationErrors
        {
            get { return validationErrors; }
        }

        public bool HasErrors
        {
            get
            {
                return
                    validationErrors != null &&
                    validationErrors.Count > 0;
            }
        }
    }
}