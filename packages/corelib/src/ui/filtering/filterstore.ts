﻿import { Criteria, Culture, localText, tryGetText, type PropertyItem } from "../../base";
import { ArgumentNullException } from "../../compat";
import { Decorators } from "../../types/decorators";
import { FilterLine } from "./filterline";

@Decorators.registerClass('Serenity.FilterStore')
export class FilterStore {

    constructor(fields: PropertyItem[]) {

        this.items = [];

        if (fields == null) {
            throw new ArgumentNullException('source');
        }

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
                displayText += ' ' + localText('Controls.FilterPanel.' +
                    (line.isOr ? 'Or' : 'And')) + ' ';
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

    declare private changed: any;
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
        this.changed && this.changed(this, {});
    }

    add_changed(value: (e: Event, a: any) => void): void {
        this.changed = delegateCombine(this.changed, value);
    }

    remove_changed(value: (e: Event, a: any) => void): void {
        this.changed = delegateRemove(this.changed, value);
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

let _mkdel = (targets: any[]): any => {
    var delegate: any = function () {
        if (targets.length === 2) {
            return targets[1].apply(targets[0], arguments);
        }
        else {
            var clone = targets.slice();
            for (var i = 0; i < clone.length; i += 2) {
                if (delegateContains(targets, clone[i], clone[i + 1])) {
                    clone[i + 1].apply(clone[i], arguments);
                }
            }
            return null;
        }
    };
    delegate._targets = targets;

    return delegate;
};

export function delegateCombine(delegate1: any, delegate2: any) {
    if (!delegate1) {
        if (!delegate2._targets) {
            return delegate2;
        }
        return delegate2;
    }
    if (!delegate2) {
        if (!delegate1._targets) {
            return delegate1;
        }
        return delegate1;
    }

    var targets1 = delegate1._targets ? delegate1._targets : [null, delegate1];
    var targets2 = delegate2._targets ? delegate2._targets : [null, delegate2];

    return _mkdel(targets1.concat(targets2));
};

export function delegateRemove(delegate1: any, delegate2: any) {
    if (!delegate1 || (delegate1 === delegate2)) {
        return null;
    }
    if (!delegate2) {
        return delegate1;
    }

    var targets = delegate1._targets;
    var object = null;
    var method;
    if (delegate2._targets) {
        object = delegate2._targets[0];
        method = delegate2._targets[1];
    }
    else {
        method = delegate2;
    }

    for (var i = 0; i < targets.length; i += 2) {
        if ((targets[i] === object) && (targets[i + 1] === method)) {
            if (targets.length === 2) {
                return null;
            }
            var t = targets.slice();
            t.splice(i, 2);
            return _mkdel(t);
        }
    }

    return delegate1;
};

export function delegateContains(targets: any[], object: any, method: any) {
    for (var i = 0; i < targets.length; i += 2) {
        if (targets[i] === object && targets[i + 1] === method) {
            return true;
        }
    }
    return false;
};
