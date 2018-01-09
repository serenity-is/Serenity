namespace Serenity {

    @Serenity.Decorators.registerEditor('Serenity.AsyncLookupEditor',
        [Serenity.ISetEditValue, Serenity.IGetEditValue, Serenity.IStringValue, Serenity.IReadOnly, Serenity.IAsyncInit])
    export class AsyncLookupEditor extends LookupEditorBase<LookupEditorOptions, any> {
        constructor(hidden: JQuery, opt: LookupEditorOptions) {
            super(hidden, opt);
        }

        getLookupKey() {
            return Q.coalesce(this.options.lookupKey, super.getLookupKey());
        }
    }
}

namespace Serenity.FilterPanels {

    @Serenity.Decorators.registerClass('Serenity.FilterPanels.FieldSelect',
        [Serenity.ISetEditValue, Serenity.IGetEditValue, Serenity.IStringValue, Serenity.IReadOnly])
    export class FieldSelect extends Select2Editor<any, PropertyItem> {
        constructor(hidden: JQuery, fields: PropertyItem[]) {
            super(hidden);

            for (var field of fields) {
                this.addOption(field.name, Q.coalesce(Q.tryGetText(field.title),
                    Q.coalesce(field.title, field.name)), field);
            }
        }

        emptyItemText() {
            if (Q.isEmptyOrNull(this.value)) {
                return Q.text('Controls.FilterPanel.SelectField');
            }

            return null;
        }

        getSelect2Options() {
            var opt = super.getSelect2Options();
            opt.allowClear = false;
            return opt;
        }
    }
}

namespace Serenity.FilterPanels {

    @Serenity.Decorators.registerClass('Serenity.FilterPanels.FieldSelect',
        [Serenity.ISetEditValue, Serenity.IGetEditValue, Serenity.IStringValue, Serenity.IReadOnly])
    export class OperatorSelect extends Select2Editor<any, FilterOperator> {
        constructor(hidden: JQuery, source: FilterOperator[]) {
            super(hidden);

            for (var op of source) {
                var title = Q.coalesce(op.title, Q.coalesce(
                    Q.tryGetText("Controls.FilterPanel.OperatorNames." + op.key), op.key));
                this.addOption(op.key, title, op);
            }

            if (source.length && source[0])
                this.value = source[0].key;
        }

        emptyItemText(): string {
            return null;
        }

        getSelect2Options() {
            var opt = super.getSelect2Options();
            opt.allowClear = false;
            return opt;
        }
    }
}

namespace Serenity {
    @Serenity.Decorators.registerClass('Serenity.BooleanFiltering')
    export class BooleanFiltering extends BaseFiltering {
        getOperators() {
            return this.appendNullableOperators([
                { key: Serenity.FilterOperators.isTrue },
                { key: Serenity.FilterOperators.isFalse }
            ]);
        }
    }
}

namespace Serenity {
    @Serenity.Decorators.registerFormatter('Serenity.BooleanFormatter')
    export class BooleanFormatter implements Slick.Formatter {
        format(ctx: Slick.FormatterContext) {

            if (ctx.value == null) {
                return '';
            }

            var text;
            if (!!ctx.value) {
                text = Q.tryGetText(this.trueText);
                if (text == null) {
                    text = this.trueText;
                    if (text == null) {
                        text = Q.coalesce(Q.tryGetText('Dialogs.YesButton'), 'Yes');
                    }
                }
            }
            else {
                text = Q.tryGetText(this.falseText);
                if (text == null) {
                    text = this.falseText;
                    if (text == null) {
                        text = Q.coalesce(Q.tryGetText('Dialogs.NoButton'), 'No');
                    }
                }
            }

			return Q.htmlEncode(text);
        }

        @Serenity.Decorators.option()
        public falseText: string;

        @Serenity.Decorators.option()
        public trueText: string;
    }
}

namespace Serenity {
    @Serenity.Decorators.registerClass('Serenity.CascadedWidgetLink')
    export class CascadedWidgetLink<TParent extends Widget<any>> {

        constructor(private parentType: { new(...args: any[]): TParent },
            private widget: Serenity.Widget<any>,
            private parentChange: (p1: TParent) => void) {
            this.bind();
            this.widget.element.bind('remove.' + (widget as any).uniqueName + 'cwh', e => {
                this.unbind();
                this.widget = null;
                this.parentChange = null;
            });
        }

        private _parentID: string;

        bind() {

            if (Q.isEmptyOrNull(this._parentID)) {
                return null;
            }

            var parent = Q.findElementWithRelativeId(this.widget.element, this._parentID)
                .tryGetWidget(this.parentType);

            if (parent != null) {
                parent.element.bind('change.' + (this.widget as any).uniqueName, () => {
                    this.parentChange(parent);
                });
                return parent;
            }
			else {
                Q.notifyError("Can't find cascaded parent element with ID: " + this._parentID + '!', '', null);
                return null;
            }
        }

        unbind() {

            if (Q.isEmptyOrNull(this._parentID)) {
                return null;
            }

			var parent = Q.findElementWithRelativeId(this.widget.element, this._parentID).tryGetWidget(this.parentType);

            if (parent != null) {
                parent.element.unbind('.' + (this.widget as any).uniqueName);
            }

			return parent;
        }

        get_parentID() {
            return this._parentID;
        }

        set_parentID(value: string) {

            if (this._parentID !== value) {
                this.unbind();
                this._parentID = value;
                this.bind();
            }
        }
    }
}

namespace Serenity {
    public interface 
}

namespace Serenity {

    @Serenity.Decorators.registerClass('Serenity.CategoryAttribute')
    export class CategoryAttribute {
        constructor(public category: string) {
        }
    }
}

namespace Serenity {

    @Serenity.Decorators.registerFormatter('Serenity.CheckboxFormatter')
    export class CheckboxFormatter implements Slick.Formatter {
        format(ctx: Slick.FormatterContext) {
            return '<span class="check-box no-float readonly ' + (!!ctx.value ? ' checked' : '') + '"></span>';
        }
    }
}

namespace Serenity {

    @Serenity.Decorators.registerClass('Serenity.CollapsibleAttribute')
    export class CollapsibleAttribute {
        constructor(public value: boolean) {
        }

        public collapsed: boolean;
    }
}

namespace Serenity {

    @Serenity.Decorators.registerClass('Serenity.CssClassAttribute')
    export class CssClassAttribute {
        constructor(public cssClass: string) {
        }
    }
}

