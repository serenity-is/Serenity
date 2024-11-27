using Ctrl = Serenity.Demo.BasicSamples.BasicSamplesPage;
using Link = Serenity.Navigation.NavigationLinkAttribute;
using Menu = Serenity.Navigation.NavigationMenuAttribute;

[assembly: Menu(7950, "Basic Samples/Grids")]
[assembly: Link(7950, "Basic Samples/Grids/Custom Links In Grid", typeof(Ctrl), action: nameof(Ctrl.CustomLinksInGrid))]
[assembly: Link(7950, "Basic Samples/Grids/Enabling Row Selection", typeof(Ctrl), action: nameof(Ctrl.EnablingRowSelection))]
[assembly: Link(7950, "Basic Samples/Grids/Grouping and Sum. In Grid", typeof(Ctrl), action: nameof(Ctrl.GroupingAndSummariesInGrid))]
[assembly: Link(7950, "Basic Samples/Grids/Grid Filtered by Criteria", typeof(Ctrl), action: nameof(Ctrl.GridFilteredByCriteria))]
[assembly: Link(7950, "Basic Samples/Grids/Initial Value for Quick Flt.", typeof(Ctrl), action: nameof(Ctrl.InitialValuesForQuickFilters))]
[assembly: Link(7950, "Basic Samples/Grids/Inline Action Buttons", typeof(Ctrl), action: nameof(Ctrl.InlineActionButtons))]
[assembly: Link(7950, "Basic Samples/Grids/Inline Image In Grid", typeof(Ctrl), action: nameof(Ctrl.InlineImageInGrid))]
[assembly: Link(7950, "Basic Samples/Grids/Removing Add Button", typeof(Ctrl), action: nameof(Ctrl.RemovingAddButton))]
[assembly: Link(7950, "Basic Samples/Grids/View Without ID", typeof(Ctrl), action: nameof(Ctrl.ViewWithoutID))]
[assembly: Link(7950, "Basic Samples/Grids/Wrapped Headers", typeof(Ctrl), action: nameof(Ctrl.WrappedHeaders))]