import { nsSerenity } from "../../base";
import { EditorProps } from "./editorwidget";
import { SelectEditor, SelectEditorOptions } from "./selecteditor";

/**
 * An editor that renders a select of years around the current year.
 * @typeParam P - Widget props type.
 */
export class DateYearEditor<P extends DateYearEditorOptions = DateYearEditorOptions> extends SelectEditor<P> {
    static override[Symbol.typeInfo] = this.registerEditor(nsSerenity);

    /**
     * Creates a date year editor.
     * @param props - Widget props.
     */
    constructor(props: EditorProps<P>) {
        super(props);

        this.updateItems();
    }

    /**
     * Returns the year options for the editor.
     * @returns The list of year strings.
     */
    override getItems() {
        const opt = this.options as DateYearEditorOptions;

        if (opt.items != null && opt.items.length >= 1) {
            return opt.items;
        }

        const years = [];
        let minYear = (new Date()).getFullYear();
        let maxYear = (new Date()).getFullYear();

        const minYearText = (opt.minYear ?? '-10').toString();
        if (minYearText.startsWith('-')) {
            minYear -= parseInt(minYearText.substring(1), 10);
        }
        else if (minYearText.startsWith('+')) {
            minYear += parseInt(minYearText.substring(1), 10);
        }
        else {
            minYear = parseInt(minYearText, 10);
        }

        const maxYearText = (opt.maxYear ?? '+10').toString();
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
            for (let i = maxYear; i >= minYear; i--) {
                years.push(i.toString());
            }
        }
        else {
            for (let i1 = minYear; i1 <= maxYear; i1++) {
                years.push(i1.toString());
            }
        }

        return years;
    }
}

/**
 * Options for the {@link DateYearEditor}.
 */
export interface DateYearEditorOptions extends SelectEditorOptions {
    /** Minimum year as an absolute value or relative offset (e.g. "-10" or "+5"). */
    minYear?: string;
    /** Maximum year as an absolute value or relative offset (e.g. "+10" or "-5"). */
    maxYear?: string;
    /** Whether years are listed in descending order. */
    descending?: boolean;
}