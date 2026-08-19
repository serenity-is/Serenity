import { cancelDialogButton, FilterPanelTexts, notifyError, nsSerenity, okDialogButton } from "../../base";
import { BaseDialog } from "../dialogs/basedialog";
import { WidgetProps } from "../widgets/widget";
import { FilterPanel } from "./filterpanel";

/**
 * A dialog that hosts a {@link FilterPanel} for editing filters.
 * @typeParam P - Widget props type.
 */
export class FilterDialog<P = {}> extends BaseDialog<P> {
    static override[Symbol.typeInfo] = this.registerClass(nsSerenity);

    declare private filterPanel: FilterPanel;

    /**
     * Creates a filter dialog.
     * @param props - Widget props.
     */
    constructor(props: WidgetProps<P>) {
        super(props);

        this.filterPanel = new FilterPanel({ element: this.findById('FilterPanel') });
        this.filterPanel.showInitialLine = true;
        this.filterPanel.showSearchButton = false;
        this.filterPanel.updateStoreOnReset = false;

        this.dialogTitle = FilterPanelTexts.DialogTitle;
    }

    /**
     * Returns the filter panel.
     * @returns The filter panel.
     */
    get_filterPanel(): FilterPanel {
        return this.filterPanel;
    }

    /**
     * Renders the dialog contents with the filter panel.
     * @returns The rendered content.
     */
    protected override renderContents(): any {
        return <div id={this.useIdPrefix().FilterPanel} />
    }

    /**
     * Returns the dialog options.
     * @returns Dialog options.
     */
    protected override getDialogOptions() {
        var opt = super.getDialogOptions();
        opt.fullScreen = "lg-down";
        return opt;
    }

    /**
     * Returns the dialog buttons.
     * @returns Dialog button definitions.
     */
    protected override getDialogButtons() {
        return [
            okDialogButton({
                click: (e) => {
                    this.filterPanel.search();
                    if (this.filterPanel.hasErrors) {
                        e.preventDefault();
                        notifyError(FilterPanelTexts.FixErrorsMessage, '', null);
                        return;
                    }
                }
            }),
            cancelDialogButton()
        ];
    }
}
