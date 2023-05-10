import { Decorators } from "../../decorators";
import { localText, trimToNull } from "@serenity-is/corelib/q";
import { FilterDialog } from "./filterdialog";
import { FilterWidgetBase } from "./filterwidgetbase";

@Decorators.registerClass('Serenity.FilterDisplayBar')
export class FilterDisplayBar extends FilterWidgetBase<any> {

    constructor(div: JQuery) {
        super(div);

        this.element.find('.cap').text(
            localText('Controls.FilterPanel.EffectiveFilter'));

        this.element.find('.edit').text(
            localText('Controls.FilterPanel.EditFilter'));

        this.element.find('.reset').attr('title',
            localText('Controls.FilterPanel.ResetFilterHint'));

        var openFilterDialog = (e: JQueryEventObject) => {
            e.preventDefault();
            var dialog = new FilterDialog();
            dialog.get_filterPanel().set_store(this.get_store());
            dialog.dialogOpen(null);
        };

        this.element.find('.edit').click(openFilterDialog);
        this.element.find('.txt').click(openFilterDialog);

        this.element.find('.reset').click(e1 => {
            e1.preventDefault();
            this.get_store().get_items().length = 0;
            this.get_store().raiseChanged();
        });
    }

    protected filterStoreChanged() {
        super.filterStoreChanged();

        var displayText = trimToNull(this.get_store().get_displayText());

        this.element.find('.current').toggle(displayText != null);
        this.element.find('.reset').toggle(displayText != null);

        if (displayText == null)
            displayText = localText('Controls.FilterPanel.EffectiveEmpty');

        this.element.find('.txt').text('[' + displayText + ']');
    }

    protected getTemplate() {
        return "<div><a class='reset'></a><a class='edit'></a>" + 
            "<div class='current'><span class='cap'></span>" +
            "<a class='txt'></a></div></div>";
    }
}
