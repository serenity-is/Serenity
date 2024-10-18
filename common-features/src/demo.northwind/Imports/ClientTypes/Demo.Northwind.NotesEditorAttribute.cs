namespace Serenity.Demo.Northwind;

public partial class NotesEditorAttribute : CustomEditorAttribute
{
    public const string Key = "Serenity.Demo.Northwind.NotesEditor";

    public NotesEditorAttribute()
        : base(Key)
    {
    }
}