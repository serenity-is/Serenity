﻿import { Fluent, localText } from "../../base";
import { Decorators } from "../../types/decorators";
import { Widget, WidgetProps } from "../widgets/widget";

export interface QuickSearchField {
    name: string;
    title: string;
}

export interface QuickSearchInputOptions {
    typeDelay?: number;
    loadingParentClass?: string;
    filteredParentClass?: string;
    onSearch?: (p1: string, p2: string, p3: (p1: boolean) => void) => void;
    fields?: QuickSearchField[];
}

@Decorators.registerClass('Serenity.QuickSearchInput')
export class QuickSearchInput<P extends QuickSearchInputOptions = QuickSearchInputOptions> extends Widget<P> {
    static override createDefaultElement() { return <input type="text" /> as HTMLInputElement; }
    declare readonly domNode: HTMLInputElement;

    declare private lastValue: string;
    declare private field: QuickSearchField;
    declare private fieldLink: HTMLElement;
    declare private fieldChanged: boolean;
    declare private timer: number;

    constructor(props: WidgetProps<P>) {
        super(props);

        this.domNode.title = localText('Controls.QuickSearch.Hint');
        this.domNode.placeholder = localText('Controls.QuickSearch.Placeholder');
        this.lastValue = (this.domNode.value ?? "").trim();

        Fluent.on(this.domNode, "keyup." + this.uniqueName, this.checkIfValueChanged.bind(this));
        Fluent.on(this.domNode, "change." + this.uniqueName, this.checkIfValueChanged.bind(this));

        this.domNode.before(<span class="quick-search-icon"><i /></span>)

        if (this.options.fields?.length) {
            this.domNode.before(
                <div class="dropdown quick-search-field">
                    <a class="quick-search-field-toggle" title={localText('Controls.QuickSearch.FieldSelection')} data-bs-toggle="dropdown" ref={el => this.fieldLink = el} />
                    <ul class="dropdown-menu">
                        {this.options.fields.map(item => <a class="dropdown-item" href="#" onClick={e => {
                            e.preventDefault();
                            this.fieldChanged = item !== this.field;
                            this.field = item;
                            this.updateInputPlaceHolder();
                            this.checkIfValueChanged();
                        }}>{item.title ?? ''}</a>)}
                    </ul>
                </div>);

            this.field = this.options.fields[0];
            this.updateInputPlaceHolder();
        }

        Fluent.on(this.domNode, "execute-search." + this.uniqueName, () => {
            if (this.timer)
                window.clearTimeout(this.timer);
            this.searchNow((this.domNode.value ?? '').trim());
        });
    }

    protected checkIfValueChanged(): void {
        if (this.domNode.classList.contains('ignore-change')) {
            return;
        }

        var value = this.get_value();
        if (value == this.lastValue && (!this.fieldChanged || !value)) {
            this.fieldChanged = false;
            return;
        }

        this.fieldChanged = false;

        if (!!this.timer) {
            window.clearTimeout(this.timer);
        }

        var self = this;
        this.timer = window.setTimeout(function () {
            self.searchNow(value);
        }, this.options.typeDelay ?? 500);

        this.lastValue = value;
    }

    get_value(): string {
        return (this.domNode.value ?? '').trim();
    }

    get_field(): QuickSearchField {
        return this.field;
    }

    set_field(value: QuickSearchField): void {
        if (this.field !== value) {
            this.fieldChanged = true;
            this.field = value;
            this.updateInputPlaceHolder();
            this.checkIfValueChanged();
        }
    }

    protected updateInputPlaceHolder() {
        this.fieldLink && (this.fieldLink.textContent = this.field?.title ?? "");
    }

    public restoreState(value: string, field: QuickSearchField) {
        this.fieldChanged = false;
        this.field = field;
        var value = (value ?? '').trim();
        this.domNode.value = value;
        this.lastValue = value;
        if (!!this.timer) {
            window.clearTimeout(this.timer);
            this.timer = null;
        }
        this.updateInputPlaceHolder();
    }

    protected searchNow(value: string) {
        this.domNode.parentElement?.classList.toggle(
            (this.options.filteredParentClass ?? 's-QuickSearchFiltered'), !!(value.length > 0));

        let klass = this.options.loadingParentClass ?? 's-QuickSearchLoading';
        this.domNode.classList.add(klass);
        this.domNode.parentElement?.classList.add(klass);

        var done = (results: any) => {
            this.domNode.classList.remove(klass);
            this.domNode.parentElement?.classList.remove(klass);

            if (!results) {
                var el = this.domNode.closest('.s-QuickSearchBar')?.querySelector<HTMLElement>('.quick-search-icon i');
                if (el) {
                    el.classList.add('s-shake-effect');
                    setTimeout(() => el.classList.remove('s-shake-effect'), 2000);
                }
            }
        };

        if (this.options.onSearch != null) {
            this.options.onSearch(this.field?.name, value, done);
        }
        else {
            done(true);
        }
    }
}