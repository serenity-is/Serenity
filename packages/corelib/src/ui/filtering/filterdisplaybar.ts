import sQuery from "@optionaldeps/squery";
import { localText } from "@serenity-is/base";
import { Decorators } from "../../decorators";
import { WidgetProps } from "../widgets/widget";
import { FilterDialog } from "./filterdialog";
import { FilterWidgetBase } from "./filterwidgetbase";

@Decorators.registerClass('Serenity.FilterDisplayBar')
export class FilterDisplayBar<P = {}> extends FilterWidgetBase<P> {

    constructor(props: WidgetProps<P>) {
        super(props);

        sQuery(this.domNode).find('.cap').text(
            localText('Controls.FilterPanel.EffectiveFilter'));

        sQuery(this.domNode).find('.edit').text(
            localText('Controls.FilterPanel.EditFilter'));

        sQuery(this.domNode).find('.reset').attr('title',
            localText('Controls.FilterPanel.ResetFilterHint'));

        var openFilterDialog = (e: Event) => {
            e.preventDefault();
            var dialog = new FilterDialog({});
            dialog.get_filterPanel().set_store(this.get_store());
            dialog.dialogOpen(null);
        };

        sQuery(this.domNode).find('.edit').click(openFilterDialog as any);
        sQuery(this.domNode).find('.txt').click(openFilterDialog as any);

        sQuery(this.domNode).find('.reset').click(e1 => {
            e1.preventDefault();
            this.get_store().get_items().length = 0;
            this.get_store().raiseChanged();
        });
    }

    protected filterStoreChanged() {
        super.filterStoreChanged();

        var displayText = this.get_store().get_displayText()?.trim() || null;

        sQuery(this.domNode).find('.current').toggle(displayText != null);
        sQuery(this.domNode).find('.reset').toggle(displayText != null);

        if (displayText == null)
            displayText = localText('Controls.FilterPanel.EffectiveEmpty');

        sQuery(this.domNode).find('.txt').text('[' + displayText + ']');
    }

    protected getTemplate() {
        return "<div><a class='reset'></a><a class='edit'></a>" +
            "<div class='current'><span class='cap'></span>" +
            "<a class='txt'></a></div></div>";
    }
}
