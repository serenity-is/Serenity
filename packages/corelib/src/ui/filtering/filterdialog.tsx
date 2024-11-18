import { Fluent, cancelDialogButton, localText, notifyError, okDialogButton } from "../../base";
import { Decorators } from "../../types/decorators";
import { BaseDialog } from "../dialogs/basedialog";
import { WidgetProps } from "../widgets/widget";
import { FilterPanel } from "./filterpanel";

@Decorators.registerClass('Serenity.FilterDialog')
export class FilterDialog<P = {}> extends BaseDialog<P> {

    declare private filterPanel: FilterPanel;

    constructor(props: WidgetProps<P>) {
        super(props);

        this.filterPanel = new FilterPanel({ element: this.findById('FilterPanel') });
        this.filterPanel.showInitialLine = true;
        this.filterPanel.showSearchButton = false;
        this.filterPanel.updateStoreOnReset = false;

        this.dialogTitle = localText('Controls.FilterPanel.DialogTitle');
    }

    get_filterPanel(): FilterPanel {
        return this.filterPanel;
    }

    protected renderContents(): any {
        return <div id={this.useIdPrefix().FilterPanel} />
    }

    protected getDialogOptions() {
        var opt = super.getDialogOptions();
        opt.fullScreen = "lg-down";
        return opt;
    }

    protected getDialogButtons() {
        return [
            okDialogButton({
                click: (e) => {
                    this.filterPanel.search();
                    if (this.filterPanel.get_hasErrors()) {
                        e.preventDefault();
                        notifyError(localText('Controls.FilterPanel.FixErrorsMessage'), '', null);
                        return;
                    }
                }
            }),
            cancelDialogButton()
        ];
    }
}
