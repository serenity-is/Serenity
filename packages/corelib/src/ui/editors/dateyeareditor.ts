import { Decorators } from "../../types/decorators";
import { EditorProps } from "../widgets/widget";
import { SelectEditor, SelectEditorOptions } from "./selecteditor";

@Decorators.registerEditor('Serenity.DateYearEditor')
export class DateYearEditor<P extends DateYearEditorOptions = DateYearEditorOptions> extends SelectEditor<P> {

    constructor(props: EditorProps<P>) {
        super(props);

        this.updateItems();
    }

    getItems() {
        var opt = this.options as DateYearEditorOptions;

        if (opt.items != null && opt.items.length >= 1) {
            return opt.items;
        }

        var years = [];
        var minYear = (new Date()).getFullYear();
        var maxYear = (new Date()).getFullYear();

        opt.minYear = (opt.minYear ?? '-10').toString();
        if (opt.minYear.startsWith('-')) {
            minYear -= parseInt(opt.minYear.substring(1), 10);
        }
        else if (opt.minYear.startsWith('+')) {
            minYear += parseInt(opt.minYear.substring(1), 10);
        }
        else {
            minYear = parseInt(opt.minYear, 10);
        }

        opt.maxYear = (opt.maxYear ?? '+10').toString();
        if (opt.maxYear.startsWith('-')) {
            maxYear -= parseInt(opt.maxYear.substr(1), 10);
        }
        else if (opt.maxYear.startsWith('+')) {
            maxYear += parseInt(opt.maxYear.substring(1), 10);
        }
        else {
            maxYear = parseInt(opt.maxYear, 10);
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