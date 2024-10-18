using Ctrl = Serenity.Demo.BasicSamples.BasicSamplesPage;
using Link = Serenity.Navigation.NavigationLinkAttribute;
using Menu = Serenity.Navigation.NavigationMenuAttribute;

[assembly: Menu(7910, "Basic Samples/Dialogs")]
[assembly: Link(7910, "Basic Samples/Dialogs/Chart in a Dialog", typeof(Ctrl), action: nameof(Ctrl.ChartInDialog))]
[assembly: Link(7910, "Basic Samples/Dialogs/Cloneable Entity Dialog", typeof(Ctrl), action: nameof(Ctrl.CloneableEntityDialog))]
[assembly: Link(7910, "Basic Samples/Dialogs/Default Values in New Dialog", typeof(Ctrl), action: nameof(Ctrl.DefaultValuesInNewDialog))]
[assembly: Link(7910, "Basic Samples/Dialogs/Dialog Boxes", typeof(Ctrl), action: nameof(Ctrl.DialogBoxes))]
[assembly: Link(7910, "Basic Samples/Dialogs/Entity Dialog as Panel", typeof(Ctrl), action: nameof(Ctrl.EntityDialogAsPanel), Url = "~/BasicSamples/EntityDialogAsPanel/11077")]
[assembly: Link(7910, "Basic Samples/Dialogs/Get Inserted Record ID", typeof(Ctrl), action: nameof(Ctrl.GetInsertedRecordId))]
[assembly: Link(7910, "Basic Samples/Dialogs/Other Form In Tab", typeof(Ctrl), action: nameof(Ctrl.OtherFormInTab))]
[assembly: Link(7910, "Basic Samples/Dialogs/Other Form, One Toolbar", typeof(Ctrl), action: nameof(Ctrl.OtherFormInTabOneBar))]
[assembly: Link(7910, "Basic Samples/Dialogs/Populate Linked Data", typeof(Ctrl), action: nameof(Ctrl.PopulateLinkedData))]
[assembly: Link(7910, "Basic Samples/Dialogs/Read-Only Dialog", typeof(Ctrl), action: nameof(Ctrl.ReadOnlyDialog))]
[assembly: Link(7910, "Basic Samples/Dialogs/Serial Auto Numbering", typeof(Ctrl), action: nameof(Ctrl.SerialAutoNumber))]