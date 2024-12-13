import { Config } from "@serenity-is/corelib";

const loaderByKey = {
    "Serenity.Demo.BasicSamples.ChangingLookupTextDialog": async () => (await import("../Editors/ChangingLookupText/ChangingLookupTextPage")).ChangingLookupTextDialog,
    "Serenity.Demo.BasicSamples.ChangingLookupTextEditor": async () => (await import("../Editors/ChangingLookupText/ChangingLookupTextPage")).ChangingLookupTextEditor,
    "Serenity.Demo.BasicSamples.ChartInDialog": async () => (await import("../Dialogs/ChartInDialog/ChartInDialogPage")).ChartInDialog,
    "Serenity.Demo.BasicSamples.CloneableEntityDialog": async () => (await import("../Dialogs/CloneableEntityDialog/CloneableEntityDialogPage")).CloneableEntityDialog,
    "Serenity.Demo.BasicSamples.CustomLinksInGrid": async () => (await import("../Grids/CustomLinksInGrid/CustomLinksInGridPage")).CustomLinksInGrid,
    "Serenity.Demo.BasicSamples.DefaultValuesInNewGrid": async () => (await import("../Dialogs/DefaultValuesInNewDialog/DefaultValuesInNewDialogPage")).DefaultValuesInNewGrid,
    "Serenity.Demo.BasicSamples.FilteredLookupDetailEditor": async () => (await import("../Editors/FilteredLookupInDetail/FilteredLookupInDetailPage")).FilteredLookupDetailEditor,
    "Serenity.Demo.BasicSamples.FilteredLookupInDetailDialog": async () => (await import("../Editors/FilteredLookupInDetail/FilteredLookupInDetailPage")).FilteredLookupInDetailDialog,
    "Serenity.Demo.BasicSamples.FilteredLookupInDetailGrid": async () => (await import("../Editors/FilteredLookupInDetail/FilteredLookupInDetailPage")).FilteredLookupInDetailGrid,
    "Serenity.Demo.BasicSamples.FilteredLookupOrderDetailDialog": async () => (await import("../Editors/FilteredLookupInDetail/FilteredLookupInDetailPage")).FilteredLookupOrderDetailDialog,
    "Serenity.Demo.BasicSamples.GetInsertedRecordIdDialog": async () => (await import("../Dialogs/GetInsertedRecordId/GetInsertedRecordIdPage")).GetInsertedRecordIdDialog,
    "Serenity.Demo.BasicSamples.GetInsertedRecordIdGrid": async () => (await import("../Dialogs/GetInsertedRecordId/GetInsertedRecordIdPage")).GetInsertedRecordIdGrid,
    "Serenity.Demo.BasicSamples.GridFilteredByCriteria": async () => (await import("../Grids/GridFilteredByCriteria/GridFilteredByCriteriaPage")).GridFilteredByCriteria,
    "Serenity.Demo.BasicSamples.GroupingAndSummariesInGrid": async () => (await import("../Grids/GroupingAndSummariesInGrid/GroupingAndSummariesInGridPage")).GroupingAndSummariesInGrid,
    "Serenity.Demo.BasicSamples.HardcodedValuesDialog": async () => (await import("../Editors/SelectWithHardcodedValues/SelectWithHardcodedValuesPage")).HardcodedValuesDialog,
    "Serenity.Demo.BasicSamples.HardcodedValuesEditor": async () => (await import("../Editors/SelectWithHardcodedValues/SelectWithHardcodedValuesPage")).HardcodedValuesEditor,
    "Serenity.Demo.BasicSamples.InitialValuesForQuickFilters": async () => (await import("../Grids/InitialValuesForQuickFilters/InitialValuesForQuickFiltersPage")).InitialValuesForQuickFilters,
    "Serenity.Demo.BasicSamples.InlineActionGrid": async () => (await import("../Grids/InlineActionButtons/InlineActionButtonsPage")).InlineActionGrid,
    "Serenity.Demo.BasicSamples.InlineImageFormatter": async () => (await import("../Grids/InlineImageInGrid/InlineImageInGridPage")).InlineImageFormatter,
    "Serenity.Demo.BasicSamples.InlineImageInGrid": async () => (await import("../Grids/InlineImageInGrid/InlineImageInGridPage")).InlineImageInGrid,
    "Serenity.Demo.BasicSamples.LookupFilterByMultipleDialog": async () => (await import("../Editors/LookupFilterByMultipleValues/LookupFilterByMultipleValuesPage")).LookupFilterByMultipleDialog,
    "Serenity.Demo.BasicSamples.LookupFilterByMultipleGrid": async () => (await import("../Editors/LookupFilterByMultipleValues/LookupFilterByMultipleValuesPage")).LookupFilterByMultipleGrid,
    "Serenity.Demo.BasicSamples.OtherFormInTabDialog": async () => (await import("../Dialogs/OtherFormInTab/OtherFormInTabPage")).OtherFormInTabDialog,
    "Serenity.Demo.BasicSamples.OtherFormInTabGrid": async () => (await import("../Dialogs/OtherFormInTab/OtherFormInTabPage")).OtherFormInTabGrid,
    "Serenity.Demo.BasicSamples.OtherFormInTabOneBarGrid": async () => (await import("../Dialogs/OtherFormInTabOneBar/OtherFormInTabOneBarPage")).OtherFormInTabOneBarGrid,
    "Serenity.Demo.BasicSamples.OtherFormOneBarDialog": async () => (await import("../Dialogs/OtherFormInTabOneBar/OtherFormInTabOneBarPage")).OtherFormOneBarDialog,
    "Serenity.Demo.BasicSamples.PopulateLinkedDataDialog": async () => (await import("../Dialogs/PopulateLinkedData/PopulateLinkedDataPage")).PopulateLinkedDataDialog,
    "Serenity.Demo.BasicSamples.PopulateLinkedDataGrid": async () => (await import("../Dialogs/PopulateLinkedData/PopulateLinkedDataPage")).PopulateLinkedDataGrid,
    "Serenity.Demo.BasicSamples.ProduceSeafoodCategoryEditor": async () => (await import("../Editors/LookupFilterByMultipleValues/LookupFilterByMultipleValuesPage")).ProduceSeafoodCategoryEditor,
    "Serenity.Demo.BasicSamples.ReadOnlyDialog": async () => (await import("../Dialogs/ReadOnlyDialog/ReadOnlyDialogPage")).ReadOnlyDialog,
    "Serenity.Demo.BasicSamples.ReadOnlyGrid": async () => (await import("../Dialogs/ReadOnlyDialog/ReadOnlyDialogPage")).ReadOnlyGrid,
    "Serenity.Demo.BasicSamples.RemovingAddButton": async () => (await import("../Grids/RemovingAddButton/RemovingAddButtonPage")).RemovingAddButton,
    "Serenity.Demo.BasicSamples.RowSelectionGrid": async () => (await import("../Grids/EnablingRowSelection/EnablingRowSelectionPage")).RowSelectionGrid,
    "Serenity.Demo.BasicSamples.SerialAutoNumberDialog": async () => (await import("../Dialogs/SerialAutoNumber/SerialAutoNumberPage")).SerialAutoNumberDialog,
    "Serenity.Demo.BasicSamples.SerialAutoNumberGrid": async () => (await import("../Dialogs/SerialAutoNumber/SerialAutoNumberPage")).SerialAutoNumberGrid,
    "Serenity.Demo.BasicSamples.StaticTextBlockDialog": async () => (await import("../Editors/StaticTextBlock/StaticTextBlockPage")).StaticTextBlockDialog,
    "Serenity.Demo.BasicSamples.ViewWithoutIDGrid": async () => (await import("../Grids/ViewWithoutID/ViewWithoutIDPage")).ViewWithoutIDGrid,
    "Serenity.Demo.BasicSamples.WrappedHeadersGrid": async () => (await import("../Grids/WrappedHeaders/WrappedHeadersPage")).WrappedHeadersGrid
}

Config.lazyTypeLoader = (function(org: any) {
    return (key: string, type: any) => loaderByKey[key]?.() || org?.(key, type);
})(Config.lazyTypeLoader);