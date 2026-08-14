import { nsSerenity } from "../../base";
import { EditorProps } from "./editorwidget";
import { SelectEditor, SelectEditorOptions } from "./selecteditor";

export class DateYearEditor<P extends DateYearEditorOptions = DateYearEditorOptions> extends SelectEditor<P> {
    static override[Symbol.typeInfo] = this.registerEditor(nsSerenity);

    constructor(props: EditorProps<P>) {
        super(props);

        this.updateItems();
    }

    override getItems() {
        var opt = this.options as DateYearEditorOptions;

        if (opt.items != null && opt.items.length >= 1) {
            return opt.items;
        }

        var years = [];
        var minYear = (new Date()).getFullYear();
        var maxYear = (new Date()).getFullYear();

        var minYearText = (opt.minYear ?? '-10').toString();
        if (minYearText.startsWith('-')) {
            minYear -= parseInt(minYearText.substring(1), 10);
        }
        else if (minYearText.startsWith('+')) {
            minYear += parseInt(minYearText.substring(1), 10);
        }
        else {
            minYear = parseInt(minYearText, 10);
        }

        var maxYearText = (opt.maxYear ?? '+10').toString();
        if (maxYearText.startsWith('-')) {
            maxYear -= parseInt(maxYearText.substring(1), 10);
        }
        else if (maxYearText.startsWith('+')) {
            maxYear += parseInt(maxYearText.substring(1), 10);
        }
        else {
            maxYear = parseInt(maxYearText, 10);
        }

        if (opt.descending) {
            for (var i = maxYear; i >= minYear; i--) {
                years.push(i.toString());
            }
        }
        else {
            for (var i1 = minYear; i1 <= maxYear; i1++) {
                years.push(i1.toString());
            }
        }

        return years;
    }
}

export interface DateYearEditorOptions extends SelectEditorOptions {
    minYear?: string;
    maxYear?: string;
    descending?: boolean;
}