namespace Serenity {

    @Decorators.registerClass('Serenity.FilterDialog')
    export class FilterDialog extends TemplatedDialog<any> {

        private filterPanel: FilterPanel;

        constructor() {
            super();

            this.filterPanel = new FilterPanel(this.byId('FilterPanel'));
            this.filterPanel.set_showInitialLine(true);
            this.filterPanel.set_showSearchButton(false);
            this.filterPanel.set_updateStoreOnReset(false);
        }

        get_filterPanel(): FilterPanel {
            return this.filterPanel;
        }

        protected getTemplate(): string {
            return '<div id="~_FilterPanel"/>';
        }

        protected getDialogOptions() {
            var opt = super.getDialogOptions();

            opt.buttons = [
                {
                    text: Q.text('Dialogs.OkButton'),
                    click: () => {
                        this.filterPanel.search();
                        if (this.filterPanel.get_hasErrors()) {
                            Q.notifyError(Q.text('Controls.FilterPanel.FixErrorsMessage'), '', null);
                            return;
                        }

                        this.dialogClose();
                    }
                },
                {
                    text: Q.text('Dialogs.CancelButton'),
                    click: () => this.dialogClose()
                }
            ];

            opt.title = Q.text('Controls.FilterPanel.DialogTitle');

            return opt;
        }
    }

}