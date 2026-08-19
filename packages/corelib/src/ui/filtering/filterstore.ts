import { classTypeInfo, Criteria, Culture, FilterPanelTexts, nsSerenity, registerType, tryGetText, type PropertyItem } from "../../base";
import { FilterLine } from "./filterline";

/**
 * Stores filter lines for a grid and builds criteria and display text from them.
 */
export class FilterStore {
    static [Symbol.typeInfo] = classTypeInfo(nsSerenity); static { registerType(this); }

    /**
     * Creates a filter store.
     * @param fields - The filterable fields.
     */
    constructor(fields: PropertyItem[]) {

        this.items = [];

        if (fields == null)
            throw new Error("FilterStore: 'fields' argument is null!");

        this.fields = fields.slice();

        this.get_fields().sort(function (x, y) {
            var titleX = tryGetText(x.title);
            if (titleX == null) {
                titleX = x.title;
                if (titleX == null)
                    titleX = x.name;
            }

            var titleY = tryGetText(y.title);
            if (titleY == null) {
                titleY = y.title;
                if (titleY == null)
                    titleY = y.name;
            }

            return Culture.stringCompare(titleX, titleY);
        });

        this.fieldByName = {};

        for (var field of fields) {
            this.get_fieldByName()[field.name] = field;
        }
    }

    /**
     * Builds a criteria expression from a list of filter lines.
     * @param items - The filter lines.
     * @returns The criteria expression.
     */
    static getCriteriaFor(items: FilterLine[]): any[] {

        if (items == null)
            return [''];

        var inParens = false;
        var currentBlock = [''];
        var isBlockOr = false;
        var criteria = [''];

        for (var i = 0; i < items.length; i++) {
            var line = items[i];

            if (line.leftParen || inParens && line.rightParen) {

                if (!Criteria.isEmpty(currentBlock)) {

                    if (inParens)
                        currentBlock = Criteria.paren(currentBlock);

                    if (isBlockOr)
                        criteria = Criteria.or(criteria, currentBlock);
                    else
                        criteria = Criteria.and(criteria, currentBlock);

                    currentBlock = [''];
                }

                inParens = false;
            }

            if (line.leftParen) {
                isBlockOr = line.isOr;
                inParens = true;
            }

            if (line.isOr)
                currentBlock = Criteria.or(currentBlock, line.criteria);
            else
                currentBlock = Criteria.and(currentBlock, line.criteria);
        }

        if (!Criteria.isEmpty(currentBlock)) {
            if (isBlockOr)
                criteria = Criteria.or(criteria, Criteria.paren(currentBlock));
            else
                criteria = Criteria.and(criteria, Criteria.paren(currentBlock));
        }

        return criteria;
    }

    /**
     * Builds the display text for a list of filter lines.
     * @param items - The filter lines.
     * @returns The display text.
     */
    static getDisplayTextFor(items: FilterLine[]): string {

        if (items == null)
            return '';

        var inParens = false;
        var displayText = '';

        for (var i = 0; i < items.length; i++) {
            var line = items[i];

            if (inParens && (line.rightParen || line.leftParen)) {
                displayText += ')';
                inParens = false;
            }

            if (displayText.length > 0) {
                displayText += ' ' + FilterPanelTexts[line.isOr ? 'Or' : 'And'] + ' ';
            }

            if (line.leftParen) {
                displayText += '(';
                inParens = true;
            }

            displayText += line.displayText;
        }

        if (inParens) {
            displayText += ')';
        }

        return displayText;
    }

    private changed: ((store: FilterStore) => void)[] = [];
    declare private displayText: string;
    declare private fields: PropertyItem[];
    declare private fieldByName: { [key: string]: PropertyItem }
    declare private items: FilterLine[];

    /**
     * Returns the filterable fields.
     * @returns The fields.
     */
    get_fields(): PropertyItem[] {
        return this.fields;
    }

    /**
     * Returns the fields by name.
     * @returns The field map.
     */
    get_fieldByName(): { [key: string]: PropertyItem } {
        return this.fieldByName;
    }

    /**
     * Returns the filter lines.
     * @returns The filter lines.
     */
    get_items(): FilterLine[] {
        return this.items;
    }

    /**
     * Notifies listeners that the store changed.
     */
    raiseChanged(): void {
        this.displayText = null;
        this.changed?.forEach(h => h(this));
    }

    /**
     * Subscribes a listener to store changes.
     * @param listener - The listener.
     */
    add_changed(listener: (store: FilterStore) => void): void {
        listener && this.changed.push(listener);
    }

    /**
     * Unsubscribes a listener from store changes.
     * @param listener - The listener.
     */
    remove_changed(listener: (store: FilterStore) => void): void {
        this.changed = this.changed?.filter(h => h !== listener);
    }

    /**
     * Returns the active criteria for the current filter lines.
     * @returns The criteria expression.
     */
    get_activeCriteria(): any[] {
        return FilterStore.getCriteriaFor(this.items);
    }

    /**
     * Returns the display text for the current filter lines.
     * @returns The display text.
     */
    get_displayText(): string {
        if (this.displayText == null)
            this.displayText = FilterStore.getDisplayTextFor(this.items);

        return this.displayText;
    }
}