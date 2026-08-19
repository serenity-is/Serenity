import { Fluent, nsSerenity, Validator } from "../../base";
import { validateOptions } from "../../compat";
import { TabsExtensions } from "../helpers/tabsextensions";
import { Toolbar, ToolButton } from "./toolbar";
import { Widget, WidgetProps } from "./widget";

/**
 * Base class for panel-style widgets that manage a form, tabs and a toolbar.
 * It wires up validation, tab initialization and toolbar buttons from the
 * panel's DOM, and is the base for {@link PropertyPanel}.
 * @typeParam P - Widget props type.
 */
export class BasePanel<P = {}> extends Widget<P> {
    static override[Symbol.typeInfo] = this.registerClass(nsSerenity);
    
    /**
     * Creates a panel, initializing the validator, tabs and toolbar.
     * @param props - Widget props forwarded to {@link Widget}.
     */
    constructor(props: WidgetProps<P>) {
        super(props);

        this.initValidator();
        this.initTabs();
        this.initToolbar();
    }

    /**
     * Destroys the tabs, toolbar and validator, then delegates to the base destroy.
     */
    override destroy() {
        TabsExtensions.destroy(this.tabs);
        this.tabs = null;

        if (this.toolbar) {
            this.toolbar.destroy();
            this.toolbar = null;
        }

        if (this.validator) {
            this.validator?.destroy?.();
            this.byId('Form').remove();
            this.validator = null;
        }

        super.destroy();
    }

    /** The initialized tabs element, if the panel has a `Tabs` div. */
    declare protected tabs: Fluent;
    /** The initialized toolbar, if the panel has a `Toolbar` div. */
    declare protected toolbar: Toolbar;
    /** The form validator, if the panel has a `Form` element. */
    declare protected validator: Validator;
    /** Whether this panel is rendered as a panel. */
    declare protected isPanel: boolean;
    /** Whether this panel is responsive. */
    declare protected responsive: boolean;

    /**
     * Triggers a `layout` event on all visible `.require-layout` elements.
     */
    public arrange(): void {
        this.element.findAll('.require-layout').forEach(el => {
            Fluent.isVisibleLike(el) && Fluent.trigger(el, "layout");
        });
    }

    /**
     * Returns the buttons to show in the panel toolbar.
     * @returns Toolbar button definitions.
     */
    protected getToolbarButtons(): ToolButton[] {
        return [];
    }

    /**
     * Returns the options used to configure the form validator.
     * @returns Validator options object.
     */
    protected getValidatorOptions(): any {
        return {};
    }

    /**
     * Initializes the tabs from the `Tabs` div, if present.
     */
    protected initTabs(): void {
        var tabsDiv = this.findById('Tabs');
        if (!tabsDiv)
            return;
        this.tabs = TabsExtensions.initialize(tabsDiv, null);
    }

    /**
     * Initializes the toolbar from the `Toolbar` div, if present.
     */
    protected initToolbar(): void {
        var toolbarDiv = this.findById('Toolbar');
        if (!toolbarDiv)
            return;
        this.toolbar = new Toolbar({ buttons: this.getToolbarButtons(), element: toolbarDiv }).init();
    }

    /**
     * Initializes the form validator from the `Form` element, if present.
     */
    protected initValidator(): void {
        var form = this.findById<HTMLFormElement>('Form');
        if (form) {
            var valOptions = this.getValidatorOptions();
            this.validator = new Validator(form, validateOptions(valOptions));
        }
    }

    /**
     * Resets all validation state on the form validator, if present.
     */
    protected resetValidation(): void {
        if (this.validator) {
            this.validator.resetAll();
        }
    }

    /**
     * Validates the form, returning whether it is valid.
     * @returns True if there is no validator or the form is valid.
     */
    protected validateForm(): boolean {
        return this.validator == null || !!this.validator.form();
    }
}

/** @deprecated use {@link BasePanel} */
export const TemplatedPanel = BasePanel;