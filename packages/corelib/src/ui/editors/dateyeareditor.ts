import { Decorators } from "../../decorators";
import { startsWith } from "@serenity-is/corelib/q";
import { SelectEditor, SelectEditorOptions } from "./selecteditor";

@Decorators.registerEditor('Serenity.DateYearEditor')
export class DateYearEditor extends SelectEditor {

    constructor(hidden: JQuery, opt: DateYearEditorOptions) {
        super(hidden, opt);

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
        if (startsWith(opt.minYear, '-')) {
            minYear -= parseInt(opt.minYear.substr(1), 10);
        }
        else if (startsWith(opt.minYear, '+')) {
            minYear += parseInt(opt.minYear.substr(1), 10);
        }
        else {
            minYear = parseInt(opt.minYear, 10);
        }

        opt.maxYear = (opt.maxYear ?? '+10').toString();
        if (startsWith(opt.maxYear, '-')) {
            maxYear -= parseInt(opt.maxYear.substr(1), 10);
        }
        else if (startsWith(opt.maxYear, '+')) {
            maxYear += parseInt(opt.maxYear.substr(1), 10);
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