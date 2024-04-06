import { Fluent, Validator } from "../../base";
import { validateOptions } from "../../q";
import { Decorators } from "../../types/decorators";
import { TabsExtensions } from "../helpers/tabsextensions";
import { TemplatedWidget } from "./templatedwidget";
import { ToolButton, Toolbar } from "./toolbar";
import { WidgetProps } from "./widget";

@Decorators.registerClass("Serenity.TemplatedPanel")
export class TemplatedPanel<P={}> extends TemplatedWidget<P> {
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
            let form = this.findById('Form');
            if (form)
                Fluent.remove(form);
            this.validator = null;
        }

        super.destroy();
    }

    protected tabs: Fluent;
    protected toolbar: Toolbar;
    protected validator: Validator;
    protected isPanel: boolean;
    protected responsive: boolean;

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
        this.toolbar = new Toolbar({ buttons: this.getToolbarButtons(), element: toolbarDiv });
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