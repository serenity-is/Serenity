import { bindThis } from "@serenity-is/domwise";
import { Fluent, nsSerenity, QuickSearchTexts } from "../../base";
import { Widget, WidgetProps } from "../widgets/widget";

/**
 * Describes a selectable quick search field.
 */
export interface QuickSearchField {
    /** Field name sent with the search request. */
    name: string;
    /** Display title shown in the field selector. */
    title: string;
}

/**
 * Arguments passed to quick search callbacks.
 */
export interface QuickSearchArgs {
    /** Name of the selected search field, if any. */
    field?: string;
    /** The search query text. */
    query: string;
    /** Callback to signal that the search completed; pass false when no results were found. */
    done: (found?: boolean) => void;
    /** When set, the search was already handled by a callback. */
    handled?: boolean;
}

/**
 * Options for the {@link QuickSearchInput} widget.
 */
export interface QuickSearchInputOptions {
    /** Delay in milliseconds before the search is triggered after typing stops. */
    typeDelay?: number;
    /** CSS class added to the parent element while a search is in progress. */
    loadingParentClass?: string;
    /** Optional list of fields the user can search within. */
    fields?: QuickSearchField[];
    /** CSS class added to the parent element when the search filters results. */
    filteredParentClass?: string;
    /** @deprecated Prefer search */
    onSearch?: (field: QuickSearchArgs["field"], query: QuickSearchArgs["query"], done: QuickSearchArgs["done"]) => void;
    /** Callback invoked before the search is executed. */
    beforeSearch?: (args: QuickSearchArgs) => void;
    /** Callback that performs the actual search. */
    search?: (args: QuickSearchArgs) => void;
}

/**
 * A text input that triggers a search after a short delay, with optional
 * field selection and loading/filtered visual states.
 * @typeParam P - Options type for the widget.
 */
export class QuickSearchInput<P extends QuickSearchInputOptions = QuickSearchInputOptions> extends Widget<P> {
    static override[Symbol.typeInfo] = this.registerClass(nsSerenity);
    
    static override createDefaultElement() { return <input type="text" /> as HTMLInputElement; }
    declare readonly domNode: HTMLInputElement;

    declare private lastValue: string;
    declare private field: QuickSearchField;
    declare private fieldLink: HTMLElement;
    declare private fieldChanged: boolean;
    declare private timer: number;

    /**
     * Creates a quick search input widget.
     * @param props - Widget props forwarded to the base widget.
     */
    constructor(props: WidgetProps<P>) {
        super(props);

        this.domNode.title = QuickSearchTexts.Hint;
        this.domNode.placeholder = QuickSearchTexts.Placeholder;
        this.lastValue = (this.domNode.value ?? "").trim();

        const boundThis = bindThis(this);
        Fluent.on(this.domNode, "keyup." + this.uniqueName, boundThis.checkIfValueChanged);
        Fluent.on(this.domNode, "change." + this.uniqueName, boundThis.checkIfValueChanged);
        Fluent.on(this.domNode, "input." + this.uniqueName, boundThis.checkIfValueChanged);

        this.domNode.before(<span class="quick-search-icon"><i /></span>)

        if (this.options.fields?.length) {
            this.domNode.before(
                <div class="dropdown quick-search-field">
                    <a class="quick-search-field-toggle" title={QuickSearchTexts.FieldSelection} data-bs-toggle="dropdown" ref={el => this.fieldLink = el} />
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
                clearTimeout(this.timer);
            this.searchNow((this.domNode.value ?? '').trim());
        });
    }

    /**
     * Checks whether the input value changed and schedules a search if so.
     */
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

        if (this.timer) {
            clearTimeout(this.timer);
        }

        var self = this;
        this.timer = window.setTimeout(function () {
            self.searchNow(value);
        }, this.options.typeDelay ?? 500);

        this.lastValue = value;
    }

    /**
     * Returns the current trimmed input value.
     * @returns The search query text.
     */
    get_value(): string {
        return (this.domNode.value ?? '').trim();
    }

    /**
     * Returns the currently selected search field.
     * @returns The active {@link QuickSearchField}.
     */
    get_field(): QuickSearchField {
        return this.field;
    }

    /**
     * Sets the active search field and refreshes the placeholder.
     * @param value - The field to select.
     */
    set_field(value: QuickSearchField): void {
        if (this.field !== value) {
            this.fieldChanged = true;
            this.field = value;
            this.updateInputPlaceHolder();
            this.checkIfValueChanged();
        }
    }

    /**
     * Updates the field selector link text with the active field title.
     */
    protected updateInputPlaceHolder() {
        this.fieldLink && (this.fieldLink.textContent = this.field?.title ?? "");
    }

    /**
     * Restores a previously persisted search state (text and field).
     * @param value - The search text to restore.
     * @param field - The search field to restore.
     */
    public restoreState(value: string, field: QuickSearchField) {
        this.fieldChanged = false;
        this.field = field;
        value = (value ?? '').trim();
        this.domNode.value = value;
        this.lastValue = value;
        if (this.timer) {
            clearTimeout(this.timer);
            this.timer = null;
        }
        this.updateInputPlaceHolder();
    }

    /**
     * Executes the search for the given value, toggling loading/filtered states
     * and invoking the configured search callbacks.
     * @param value - The search query text.
     */
    protected searchNow(value: string) {
        this.domNode.parentElement?.classList.toggle(
            (this.options.filteredParentClass ?? 's-QuickSearchFiltered'), value.length > 0);

        let klass = this.options.loadingParentClass ?? 's-QuickSearchLoading';
        this.domNode.classList.add(klass);
        this.domNode.parentElement?.classList.add(klass);

        var done = (results: boolean) => {
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

        const args = { field: this.field?.name, query: value, done };
        this.options.beforeSearch?.(args);
        if (this.options.onSearch != null) {
            this.options.onSearch(args.field, args.query, args.done);
        }
        else if (this.options.search != null) {
            this.options.search(args);
        }
        else {
            done(true);
        }
    }
}