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

        /// <summary>
        /// Gets or sets the post handler.
        /// </summary>
        /// <value>
        /// The post handler.
        /// </value>
        public Action<Row<TFields>> PostHandler
        {
            get { return postHandler; }
            set { postHandler = value; }
        }

        /// <summary>
        /// Determines whether [is field changed] [the specified field].
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>
        ///   <c>true</c> if [is field changed] [the specified field]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsFieldChanged(Field field)
        {
            return (originalValues != null &&
                    field.IndexCompare(originalValues, this) != 0);
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
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

        /// <summary>
        /// Begins an edit on an object.
        /// </summary>
        public void BeginEdit()
        {
            TrackAssignments = true;

            if (originalValues == null)
                originalValues = CloneRow();
        }

        /// <summary>
        /// Discards changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit" /> call.
        /// </summary>
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

        /// <summary>
        /// Pushes changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit" /> or <see cref="M:System.ComponentModel.IBindingList.AddNew" /> call into the underlying object.
        /// </summary>
        /// <exception cref="Exception">Please fix the marked fields on the row.</exception>
        public void EndEdit()
        {
            if (postHandler != null &&
                originalValues != null)
            {
                if (insidePostHandler > 0)
                    return;

                insidePostHandler++;
                try
                {
                    ClearValidationErrors();
                    postHandler(this);
                    if (HasErrors)
                        throw new Exception("Please fix the marked fields on the row.");
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

        /// <summary>
        /// Gets a value indicating whether this instance is editing.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is editing; otherwise, <c>false</c>.
        /// </value>
        public bool IsEditing => originalValues != null;

        /// <summary>
        /// Gets the original values.
        /// </summary>
        /// <value>
        /// The original values.
        /// </value>
        public Row<TFields> OriginalValues => originalValues ?? this;

        /// <summary>
        /// Gets the previous values.
        /// </summary>
        /// <value>
        /// The previous values.
        /// </value>
        public Row<TFields> PreviousValues => previousValues ?? this;

        /// <summary>
        /// Gets a value indicating whether this instance has post handler.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has post handler; otherwise, <c>false</c>.
        /// </value>
        public bool HasPostHandler => postHandler != null;

        /// <summary>
        /// The post ended
        /// </summary>
        public EventHandler PostEnded;

        /// <summary>
        /// Adds the validation error.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="error">The error.</param>
        public void AddValidationError(string propertyName, string error)
        {
            if (validationErrors == null)
                validationErrors = new Dictionary<string, string>();

            validationErrors[propertyName ?? string.Empty] = error;
        }

        /// <summary>
        /// Clears the validation errors.
        /// </summary>
        public void ClearValidationErrors()
        {
            if (validationErrors != null &&
                validationErrors.Count > 0)
            {
                validationErrors.Clear();
            }
        }

        /// <summary>
        /// Removes the validation error.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void RemoveValidationError(string propertyName)
        {
            if (validationErrors != null)
                validationErrors.Remove(propertyName ?? string.Empty);
        }

        /// <summary>
        /// Gets the validation errors.
        /// </summary>
        /// <value>
        /// The validation errors.
        /// </value>
        public IDictionary<string, string> ValidationErrors => validationErrors;

        /// <summary>
        /// Gets a value indicating whether this instance has errors.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </value>
        public bool HasErrors => validationErrors != null &&
                    validationErrors.Count > 0;
    }
}