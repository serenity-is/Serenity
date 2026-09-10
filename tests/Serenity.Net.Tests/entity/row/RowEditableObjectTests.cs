namespace Serenity.Data;

public class RowEditableObjectTests
{
    [Fact]
    public void BeginEdit_SetsIsEditing_CreatesOriginalValues_AndEnablesTracking()
    {
        var row = new IdNameRow { Name = "Test" };
        var editable = (IEditableRow)row;
        ((IRow)row).TrackAssignments = false;

        ((IEditableObject)row).BeginEdit();

        Assert.True(editable.IsEditing);
        Assert.NotNull(editable.OriginalValues);
        Assert.NotSame(row, editable.OriginalValues);
        Assert.True(((IRow)row).TrackAssignments);
    }

    [Fact]
    public void BeginEdit_Twice_KeepsSameOriginalValuesInstance()
    {
        var row = new IdNameRow { Name = "Test" };
        var editable = (IEditableRow)row;

        ((IEditableObject)row).BeginEdit();
        var first = editable.OriginalValues;

        ((IEditableObject)row).BeginEdit();

        Assert.Same(first, editable.OriginalValues);
    }

    [Fact]
    public void EndEdit_ClearsEditingState_AndOriginalValuesReturnsSelf()
    {
        var row = new IdNameRow { Name = "Test" };
        var editable = (IEditableRow)row;

        ((IEditableObject)row).BeginEdit();
        Assert.True(editable.IsEditing);
        Assert.NotSame(row, editable.OriginalValues);

        ((IEditableObject)row).EndEdit();

        Assert.False(editable.IsEditing);
        Assert.Same(row, editable.OriginalValues);
    }

    [Fact]
    public void EndEdit_WithoutBeginEdit_IsNoOp()
    {
        var row = new IdNameRow { Name = "Test" };
        var editable = (IEditableRow)row;

        ((IEditableObject)row).EndEdit();

        Assert.False(editable.IsEditing);
        Assert.Same(row, editable.OriginalValues);
        Assert.Equal("Test", row.Name);
    }

    [Fact]
    public void CancelEdit_RestoresOriginalValues_AndAssignmentState()
    {
        var row = new IdNameRow { Name = "Original" };
        var fields = row.GetFields();

        ((IEditableObject)row).BeginEdit();
        row.Name = "Changed";
        Assert.Equal("Changed", row.Name);
        Assert.True(row.IsAssigned(fields.Name));

        ((IEditableObject)row).CancelEdit();

        Assert.Equal("Original", row.Name);
        Assert.True(row.IsAssigned(fields.Name));
        Assert.False(((IEditableRow)row).IsEditing);
        Assert.Same(row, ((IEditableRow)row).OriginalValues);
    }

    [Fact]
    public void CancelEdit_RestoresUnassignedFieldState()
    {
        var row = new IdNameRow();
        var fields = row.GetFields();

        ((IEditableObject)row).BeginEdit();
        row.Name = "Changed";
        Assert.True(row.IsAssigned(fields.Name));

        ((IEditableObject)row).CancelEdit();

        Assert.Null(row.Name);
        Assert.False(row.IsAssigned(fields.Name));
    }

    [Fact]
    public void CancelEdit_WithoutBeginEdit_IsNoOp()
    {
        var row = new IdNameRow { Name = "Test" };

        ((IEditableObject)row).CancelEdit();

        Assert.Equal("Test", row.Name);
        Assert.False(((IEditableRow)row).IsEditing);
    }

    [Fact]
    public void IsFieldChanged_FalseBeforeEdit_TrueAfterChange_FalseForUnchanged()
    {
        var row = new IdNameRow { ID = 1, Name = "Test" };
        var editable = (IEditableRow)row;
        var fields = row.GetFields();

        Assert.False(editable.IsFieldChanged(fields.Name));

        ((IEditableObject)row).BeginEdit();
        Assert.False(editable.IsFieldChanged(fields.Name));

        row.Name = "Changed";
        Assert.True(editable.IsFieldChanged(fields.Name));
        Assert.False(editable.IsFieldChanged(fields.ID));
    }

    [Fact]
    public void IsAnyFieldChanged_FalseWhenNotEditing_TrueWhenChanged()
    {
        var row = new IdNameRow { Name = "Test" };
        var editable = (IEditableRow)row;

        Assert.False(editable.IsAnyFieldChanged);

        ((IEditableObject)row).BeginEdit();
        Assert.False(editable.IsAnyFieldChanged);

        row.Name = "Changed";
        Assert.True(editable.IsAnyFieldChanged);
    }

    [Fact]
    public void PostHandler_HasPostHandler_FalseByDefault_SetGetWorks()
    {
        var row = new IdNameRow();
        var editable = (IEditableRow)row;

        Assert.False(editable.HasPostHandler);

        Action<IRow> handler = r => { };
        editable.PostHandler = handler;

        Assert.True(editable.HasPostHandler);
        Assert.Same(handler, editable.PostHandler);
    }

    [Fact]
    public void EndEdit_WithPostHandler_InvokesHandlerOnce_AndClearsOriginalValues()
    {
        var row = new IdNameRow { Name = "Test" };
        var editable = (IEditableRow)row;
        var calls = new List<IRow>();
        editable.PostHandler = r => calls.Add(r);

        ((IEditableObject)row).BeginEdit();
        ((IEditableObject)row).EndEdit();

        Assert.Single(calls);
        Assert.Same(row, calls[0]);
        Assert.False(editable.IsEditing);
        Assert.Same(row, editable.OriginalValues);
    }

    [Fact]
    public void EndEdit_WithPostHandler_WithoutBeginEdit_DoesNotInvokeHandler()
    {
        var row = new IdNameRow { Name = "Test" };
        var editable = (IEditableRow)row;
        var calls = new List<IRow>();
        editable.PostHandler = r => calls.Add(r);

        ((IEditableObject)row).EndEdit();

        Assert.Empty(calls);
        Assert.False(editable.IsEditing);
    }

    [Fact]
    public void EndEdit_WithPostHandler_AndValidationError_Throws()
    {
        var row = new IdNameRow { Name = "Test" };
        var editable = (IEditableRow)row;
        var postEndedFired = false;
        editable.PostEnded += (s, e) => postEndedFired = true;
        editable.PostHandler = r =>
            ((IEditableRow)r).AddValidationError("Name", "Name is invalid");

        ((IEditableObject)row).BeginEdit();

        var exception = Assert.Throws<Exception>(() => ((IEditableObject)row).EndEdit());

        Assert.Equal("Please fix the marked fields on the row.", exception.Message);
        Assert.True(editable.HasErrors);
        Assert.NotNull(editable.ValidationErrors);
        Assert.Equal("Name is invalid", editable.ValidationErrors["Name"]);
        Assert.True(editable.IsEditing);
        Assert.False(postEndedFired);
    }

    [Fact]
    public void EndEdit_ReentrantCallFromPostHandler_DoesNotLoop()
    {
        var row = new IdNameRow { Name = "Test" };
        var editable = (IEditableRow)row;
        var calls = new List<IRow>();
        editable.PostHandler = r =>
        {
            calls.Add(r);
            ((IEditableObject)r).EndEdit();
        };

        ((IEditableObject)row).BeginEdit();
        ((IEditableObject)row).EndEdit();

        Assert.Single(calls);
        Assert.False(editable.IsEditing);
        Assert.Same(row, editable.OriginalValues);
    }

    [Fact]
    public void PostEnded_FiresAfterSuccessfulEndEdit()
    {
        var row = new IdNameRow { Name = "Test" };
        var editable = (IEditableRow)row;
        var senders = new List<object?>();
        editable.PostEnded += (s, e) => senders.Add(s);
        editable.PostHandler = r => { };

        ((IEditableObject)row).BeginEdit();
        ((IEditableObject)row).EndEdit();

        Assert.Single(senders);
        Assert.Same(row, senders[0]);
    }

    [Fact]
    public void PostEnded_DoesNotFire_WithoutPostHandler()
    {
        var row = new IdNameRow { Name = "Test" };
        var editable = (IEditableRow)row;
        var fired = false;
        editable.PostEnded += (s, e) => fired = true;

        ((IEditableObject)row).BeginEdit();
        ((IEditableObject)row).EndEdit();

        Assert.False(fired);
        Assert.False(editable.IsEditing);
    }

    [Fact]
    public void EndEdit_WithoutPostHandler_ClearsValidationErrors()
    {
        var row = new IdNameRow();
        var editable = (IEditableRow)row;
        editable.AddValidationError("Name", "Error");

        ((IEditableObject)row).EndEdit();

        Assert.False(editable.HasErrors);
    }

    [Fact]
    public void PropertyChanged_Subscribe_CreatesPreviousValuesSnapshot()
    {
        var row = new IdNameRow();
        var editable = (IEditableRow)row;
        var notify = (INotifyPropertyChanged)row;

        Assert.Same(row, editable.PreviousValues);

        PropertyChangedEventHandler handler = (s, e) => { };
        notify.PropertyChanged += handler;

        Assert.NotNull(editable.PreviousValues);
        Assert.NotSame(row, editable.PreviousValues);

        notify.PropertyChanged -= handler;
    }

    [Fact]
    public void PropertyChanged_FieldSet_RaisesEventWithPropertyName_AndUnsubscribeWorks()
    {
        var row = new IdNameRow();
        var notify = (INotifyPropertyChanged)row;
        var received = new List<PropertyChangedEventArgs>();
        PropertyChangedEventHandler handler = (s, e) => received.Add(e);

        notify.PropertyChanged += handler;
        row.Name = "Test";

        Assert.Single(received);
        Assert.Equal("Name", received[0].PropertyName);

        notify.PropertyChanged -= handler;
        row.Name = "Test2";

        Assert.Single(received);
    }

    [Fact]
    public void ValidationErrors_FullLifecycle_Works()
    {
        var row = new IdNameRow();
        var editable = (IEditableRow)row;

        Assert.False(editable.HasErrors);
        Assert.Null(editable.ValidationErrors);

        editable.AddValidationError("Name", "Error 1");
        Assert.True(editable.HasErrors);
        Assert.NotNull(editable.ValidationErrors);
        Assert.Equal("Error 1", editable.ValidationErrors["Name"]);

        editable.AddValidationError(null, "Error 2");
        Assert.Equal("Error 2", editable.ValidationErrors[""]);

        editable.RemoveValidationError("NonExistent");
        Assert.True(editable.HasErrors);

        editable.RemoveValidationError("Name");
        Assert.False(editable.ValidationErrors.ContainsKey("Name"));
        Assert.True(editable.HasErrors);

        editable.RemoveValidationError(null);
        Assert.False(editable.HasErrors);

        editable.AddValidationError("Name", "Error 3");
        Assert.True(editable.HasErrors);

        editable.ClearValidationErrors();
        Assert.False(editable.HasErrors);
        Assert.NotNull(editable.ValidationErrors);
        Assert.Empty(editable.ValidationErrors);
    }

    [Fact]
    public void OriginalValues_AndPreviousValues_ReturnSelf_WhenNull()
    {
        var row = new IdNameRow();
        var editable = (IEditableRow)row;

        Assert.Same(row, editable.OriginalValues);
        Assert.Same(row, editable.PreviousValues);
    }
}
