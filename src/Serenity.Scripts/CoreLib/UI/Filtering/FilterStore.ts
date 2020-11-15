namespace Serenity {

    @Decorators.registerClass('FilterStore')
    export class FilterStore {

        constructor(fields: PropertyItem[]) {

            this.items = [];

            if (fields == null) {
                throw new Q.ArgumentNullException('source');
            }

            this.fields = fields.slice();

            this.get_fields().sort(function (x, y) {
                var titleX = Q.tryGetText(x.title);
                if (titleX == null) {
                    titleX = x.title;
                    if (titleX == null)
                        titleX = x.name;
                }

                var titleY = Q.tryGetText(y.title);
                if (titleY == null) {
                    titleY = y.title;
                    if (titleY == null)
                        titleY = y.name;
                }

                return Q.Culture.stringCompare(titleX, titleY);
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

                    if (!Serenity.Criteria.isEmpty(currentBlock)) {

                        if (inParens)
                            currentBlock = Serenity.Criteria.paren(currentBlock);

                        if (isBlockOr)
                            criteria = Serenity.Criteria.join(criteria,
                                'or', currentBlock);
                        else
                            criteria = Serenity.Criteria.join(criteria,
                                'and', currentBlock);

                        currentBlock = [''];
                    }

                    inParens = false;
                }

                if (line.leftParen) {
                    isBlockOr = line.isOr;
                    inParens = true;
                }

                if (line.isOr)
                    currentBlock = Serenity.Criteria.join(currentBlock,
                        'or', line.criteria);
                else
                    currentBlock = Serenity.Criteria.join(currentBlock,
                        'and', line.criteria);
            }

            if (!Serenity.Criteria.isEmpty(currentBlock)) {
                if (isBlockOr)
                    criteria = Serenity.Criteria.join(criteria,
                        'or', Serenity.Criteria.paren(currentBlock));
                else
                    criteria = Serenity.Criteria.join(criteria,
                        'and', Serenity.Criteria.paren(currentBlock));
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
                    displayText += ' ' + Q.text('Controls.FilterPanel.' +
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

        private changed: any;
        private displayText: string;
        private fields: PropertyItem[];
        private fieldByName: Q.Dictionary<PropertyItem>
        private items: FilterLine[];

        get_fields(): PropertyItem[] {
            return this.fields;
        }

        get_fieldByName(): Q.Dictionary<PropertyItem> {
            return this.fieldByName;
        }

        get_items(): FilterLine[] {
            return this.items;
        }

        raiseChanged(): void {
            this.displayText = null;
            this.changed && this.changed(this, {});
        }

        add_changed(value: (e: JQueryEventObject, a: any) => void): void {
            this.changed = Q.delegateCombine(this.changed, value);
        }

        remove_changed(value: (e: JQueryEventObject, a: any) => void): void {
            this.changed = Q.delegateRemove(this.changed, value);
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
}