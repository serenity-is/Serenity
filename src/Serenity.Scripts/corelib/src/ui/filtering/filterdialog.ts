import { Decorators } from "../../decorators";
import { notifyError, text } from "@serenity-is/corelib/q";
import { TemplatedDialog } from "../dialogs/templateddialog";
import { FilterPanel } from "./filterpanel";

@Decorators.registerClass('Serenity.FilterDialog')
export class FilterDialog extends TemplatedDialog<any> {

    private filterPanel: FilterPanel;

    constructor() {
        super();

        this.filterPanel = new FilterPanel(this.byId('FilterPanel'));
        this.filterPanel.set_showInitialLine(true);
        this.filterPanel.set_showSearchButton(false);
        this.filterPanel.set_updateStoreOnReset(false);

        this.dialogTitle = text('Controls.FilterPanel.DialogTitle');
    }

    get_filterPanel(): FilterPanel {
        return this.filterPanel;
    }

    protected getTemplate(): string {
        return '<div id="~_FilterPanel"/>';
    }

    protected getDialogButtons() {
        return [
            {
                text: text('Dialogs.OkButton'),
                click: () => {
                    this.filterPanel.search();
                    if (this.filterPanel.get_hasErrors()) {
                        notifyError(text('Controls.FilterPanel.FixErrorsMessage'), '', null);
                        return;
                    }

                    this.dialogClose();
                }
            },
            {
                text: text('Dialogs.CancelButton'),
                click: () => this.dialogClose()
            }
        ];
    }
}
