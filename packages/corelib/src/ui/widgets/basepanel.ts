import { Fluent, Validator } from "../../base";
import { validateOptions } from "../../compat";
import { Decorators } from "../../types/decorators";
import { TabsExtensions } from "../helpers/tabsextensions";
import { ToolButton, Toolbar } from "./toolbar";
import { Widget, WidgetProps } from "./widget";

@Decorators.registerClass("Serenity.BasePanel")
export class BasePanel<P={}> extends Widget<P> {
    constructor(props: WidgetProps<P>) {
        super(props);
        
        this.initValidator();
        this.initTabs();
        this.initToolbar();
    }

    destroy() {
        TabsExtensions.destroy(this.tabs);
        this.tabs = null;

        if (this.toolbar) {
            this.toolbar.destroy();
            this.toolbar = null;
        }

        if (this.validator) {
            this.validator.destroy();
            this.byId('Form').remove();
            this.validator = null;
        }

        super.destroy();
    }

    declare protected tabs: Fluent;
    declare protected toolbar: Toolbar;
    declare protected validator: Validator;
    declare protected isPanel: boolean;
    declare protected responsive: boolean;

    public arrange(): void {
        this.element.findAll('.require-layout').forEach(el => {
            Fluent.isVisibleLike(el) && Fluent.trigger(el, "layout");
        });
    }

    protected getToolbarButtons(): ToolButton[] {
        return [];
    }

    protected getValidatorOptions(): any {
        return {};
    }

    protected initTabs(): void {
        var tabsDiv = this.findById('Tabs');
        if (!tabsDiv)
            return;
        this.tabs = TabsExtensions.initialize(tabsDiv, null);
    }

    protected initToolbar(): void {
        var toolbarDiv = this.findById('Toolbar');
        if (!toolbarDiv)
            return;
        this.toolbar = new Toolbar({ buttons: this.getToolbarButtons(), element: toolbarDiv }).init();
    }

    protected initValidator(): void {
        var form = this.findById<HTMLFormElement>('Form');
        if (form) {
            var valOptions = this.getValidatorOptions();
            this.validator = new Validator(form, validateOptions(valOptions));
        }
    }

    protected resetValidation(): void {
        if (this.validator) {
            this.validator.resetAll();
        }
    }

    protected validateForm(): boolean {
        return this.validator == null || !!this.validator.form();
    }
}

/** @deprecated use BasePanel */
export const TemplatedPanel = BasePanel;