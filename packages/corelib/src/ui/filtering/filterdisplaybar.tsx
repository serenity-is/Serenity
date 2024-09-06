import { Fluent, localText } from "../../base";
import { Decorators } from "../../types/decorators";
import { FilterDialog } from "./filterdialog";
import { FilterWidgetBase } from "./filterwidgetbase";

@Decorators.registerClass('Serenity.FilterDisplayBar')
export class FilterDisplayBar<P = {}> extends FilterWidgetBase<P> {

    protected renderContents(): any {
        var openFilterDialog = (e: Event) => {
            e.preventDefault();
            var dialog = new FilterDialog({});
            dialog.get_filterPanel().set_store(this.get_store());
            dialog.dialogOpen(null);
        };

        return (
            <div>
                <a class="reset" title={localText('Controls.FilterPanel.ResetFilterHint')} onClick={(e) => {
                    e.preventDefault();
                    this.get_store().get_items().length = 0;
                    this.get_store().raiseChanged();
                }}></a>
                <a class="edit" onClick={openFilterDialog}>{localText('Controls.FilterPanel.EditFilter')}</a>
                <div class="current">
                    <span class="cap">{localText('Controls.FilterPanel.EffectiveFilter')}</span>
                    <a class="txt" onClick={openFilterDialog}></a>
                </div>
            </div>
        );
    }

    protected filterStoreChanged() {
        super.filterStoreChanged();

        var displayText = this.get_store().get_displayText()?.trim() || null;

        this.element.findFirst('.current').toggle(displayText != null);
        this.element.findFirst('.reset').toggle(displayText != null);

        if (displayText == null)
            displayText = localText('Controls.FilterPanel.EffectiveEmpty');

        this.element.findFirst('.txt').text('[' + displayText + ']');
    }
}
