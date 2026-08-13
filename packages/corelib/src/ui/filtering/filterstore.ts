import { classTypeInfo, Criteria, Culture, FilterPanelTexts, nsSerenity, registerType, tryGetText, type PropertyItem } from "../../base";
import { FilterLine } from "./filterline";

export class FilterStore {
    static [Symbol.typeInfo] = classTypeInfo(nsSerenity); static { registerType(this); }

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

    get_fields(): PropertyItem[] {
        return this.fields;
    }

    get_fieldByName(): { [key: string]: PropertyItem } {
        return this.fieldByName;
    }

    get_items(): FilterLine[] {
        return this.items;
    }

    raiseChanged(): void {
        this.displayText = null;
        this.changed?.forEach(h => h(this));
    }

    add_changed(listener: (store: FilterStore) => void): void {
        listener && this.changed.push(listener);
    }

    remove_changed(listener: (store: FilterStore) => void): void {
        this.changed = this.changed?.filter(h => h !== listener);
    }

    get_activeCriteria(): any[] {
        return FilterStore.getCriteriaFor(this.items);
    }

    get_displayText(): string {
        if (this.displayText == null)
            this.displayText = FilterStore.getDisplayTextFor(this.items);

        return this.displayText;
    }
}