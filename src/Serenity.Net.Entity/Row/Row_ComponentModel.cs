namespace Serenity.Data;

public abstract partial class Row<TFields> : IEditableRow
{
    internal int insidePostHandler;
    internal Row<TFields> originalValues;
    internal Row<TFields> previousValues;
    internal PropertyChangedEventHandler propertyChanged;
    internal Action<IRow> postHandler;
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

    Action<IRow> IEditableRow.PostHandler
    {
        get { return postHandler; }
        set { postHandler = value; }
    }

    bool IEditableRow.IsFieldChanged(Field field)
    {
        return (originalValues != null &&
                field.IndexCompare(originalValues, this) != 0);
    }

    event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
    {
        add
        {
            propertyChanged += value;
            previousValues ??= CloneRow();
        }
        remove
        {
            propertyChanged -= value;
        }
    }

    /// <summary>
    /// Begins an edit on an object.
    /// </summary>
    void IEditableObject.BeginEdit()
    {
        ((IRow)this).TrackAssignments = true;

        originalValues ??= CloneRow();
    }

    /// <summary>
    /// Discards changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit" /> call.
    /// </summary>
    void IEditableObject.CancelEdit()
    {
        if (originalValues != null)
        {
            var original = originalValues;

            originalValues = null;

            for (int i = 0; i < fields.Count; i++)
                fields[i].CopyNoAssignment(original, this);

            assignedFields = original.assignedFields;

            ((IEditableRow)this).ClearValidationErrors();
        }
    }

    /// <summary>
    /// Pushes changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit" /> or <see cref="M:System.ComponentModel.IBindingList.AddNew" /> call into the underlying object.
    /// </summary>
    /// <exception cref="Exception">Please fix the marked fields on the row.</exception>
    void IEditableObject.EndEdit()
    {
        if (postHandler != null &&
            originalValues != null)
        {
            if (insidePostHandler > 0)
                return;

            insidePostHandler++;
            try
            {
                ((IEditableRow)this).ClearValidationErrors();
                postHandler(this);
                if (((IEditableRow)this).HasErrors)
                    throw new Exception("Please fix the marked fields on the row.");
                originalValues = null;
            }
            finally
            {
                insidePostHandler--;
            }

            postEnded?.Invoke(this, new EventArgs());
        }
        else
        {
            originalValues = null;
            ((IEditableRow)this).ClearValidationErrors();
        }
    }


    /// <summary>
    /// Gets a value indicating whether this instance is any field changed.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is any field changed; otherwise, <c>false</c>.
    /// </value>
    bool IEditableRow.IsAnyFieldChanged
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

    bool IEditableRow.IsEditing => originalValues != null;

    IRow IEditableRow.OriginalValues => originalValues ?? this;

    IRow IEditableRow.PreviousValues => previousValues ?? this;

    bool IEditableRow.HasPostHandler => postHandler != null;

    private EventHandler postEnded;

    event EventHandler IEditableRow.PostEnded 
    { 
        add => postEnded += value; 
        remove => postEnded -= value;
    }

    void IEditableRow.AddValidationError(string propertyName, string error)
    {
        validationErrors ??= new Dictionary<string, string>();
        validationErrors[propertyName ?? string.Empty] = error;
    }

    void IEditableRow.ClearValidationErrors()
    {
        if (validationErrors != null &&
            validationErrors.Count > 0)
        {
            validationErrors.Clear();
        }
    }

    void IEditableRow.RemoveValidationError(string propertyName)
    {
        validationErrors?.Remove(propertyName ?? string.Empty);
    }

    IDictionary<string, string> IEditableRow.ValidationErrors => validationErrors;

    bool IEditableRow.HasErrors => validationErrors != null &&
            validationErrors.Count > 0;
}