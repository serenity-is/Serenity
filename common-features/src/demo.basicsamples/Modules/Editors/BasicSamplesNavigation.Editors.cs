using Ctrl = Serenity.Demo.BasicSamples.BasicSamplesPage;
using Link = Serenity.Navigation.NavigationLinkAttribute;
using Menu = Serenity.Navigation.NavigationMenuAttribute;

[assembly: Menu(7930, "Basic Samples/Editors")]
[assembly: Link(7930, "Basic Samples/Editors/Changing Lookup Text", typeof(Ctrl), action: nameof(Ctrl.ChangingLookupText))]
[assembly: Link(7930, "Basic Samples/Editors/Filtered Lookup in Detail.", typeof(Ctrl), action: nameof(Ctrl.FilteredLookupInDetailDialog))]
[assembly: Link(7930, "Basic Samples/Editors/Lookup Filter by Multi Val.", typeof(Ctrl), action: nameof(Ctrl.LookupFilterByMultipleValues))]
[assembly: Link(7930, "Basic Samples/Editors/Select with Hardcod.Vals.", typeof(Ctrl), action: nameof(Ctrl.SelectWithHardcodedValues))]
[assembly: Link(7930, "Basic Samples/Editors/Static Text Block", typeof(Ctrl), action: nameof(Ctrl.StaticTextBlock))]
