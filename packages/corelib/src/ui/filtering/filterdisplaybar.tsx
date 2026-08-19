import { faIcon, FilterPanelTexts, nsSerenity } from "../../base";
import { type ToolButton, type ToolButtonProps } from "../widgets/toolbar";
import { FilterDialog } from "./filterdialog";
import { FilterWidgetBase } from "./filterwidgetbase";

/**
 * A bar that displays the effective filter and lets users edit or reset it.
 * @typeParam P - Widget props type.
 */
export class FilterDisplayBar<P = {}> extends FilterWidgetBase<P> {
    static override[Symbol.typeInfo] = this.registerClass(nsSerenity);

    /**
     * Renders the filter display bar contents.
     * @returns The rendered content.
     */
    protected override renderContents(): any {
        var openFilterDialog = (e: Event) => {
            e.preventDefault();
            var dialog = new FilterDialog({});
            dialog.get_filterPanel().set_store(this.get_store());
            dialog.dialogOpen(null);
        };

        return (
            <div>
                <a class="reset" title={FilterPanelTexts.ResetFilterHint} onClick={(e) => {
                    e.preventDefault();
                    this.get_store().get_items().length = 0;
                    this.get_store().raiseChanged();
                }}></a>
                <a class="edit" onClick={openFilterDialog}>{FilterPanelTexts.EditFilter}</a>
                <div class="current">
                    <span class="cap">{FilterPanelTexts.EffectiveFilter}</span>
                    <a class="txt" onClick={openFilterDialog}></a>
                </div>
            </div>
        );
    }

    /**
     * Updates the display when the filter store changes.
     */
    protected override filterStoreChanged() {
        super.filterStoreChanged();

        var displayText = this.get_store().get_displayText()?.trim() || null;

        this.element.findFirst('.current').toggle(displayText != null);
        this.element.findFirst('.reset').toggle(displayText != null);

        if (displayText == null)
            displayText = FilterPanelTexts.EffectiveEmpty;

        this.element.findFirst('.txt').text('[' + displayText + ']');
    }

    /**
     * Creates a toolbar button that opens the filter dialog.
     * @param opt - Tool button overrides.
     * @returns Tool button definition.
     */
    public static createToolButton(opt: Partial<ToolButtonProps>): ToolButton {
        return {
            hint: FilterPanelTexts.EditFilter,
            action: 'edit-filter',
            cssClass: "edit-filter-button",
            icon: faIcon("filter"),
            ...opt
        };
    }
}
