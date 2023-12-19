import { Decorators } from "../../decorators";
import { validateOptions } from "../../q";
import { TemplatedWidget } from "./templatedwidget";
import { Toolbar, ToolButton } from "./toolbar";
import { WidgetProps } from "./widget";

@Decorators.registerClass("Serenity.TemplatedPanel")
export class TemplatedPanel<P={}> extends TemplatedWidget<P> {
    constructor(container: JQuery, props?: P) {
        super(container, props);

        this.initValidator();
        this.initTabs();
        this.initToolbar();
    }

    destroy() {
        if (this.tabs) {
            (this.tabs as any).tabs?.('destroy');
            this.tabs = null;
        }

        if (this.toolbar) {
            this.toolbar.destroy();
            this.toolbar = null;
        }

        if (this.validator) {
            this.byId('Form').remove();
            this.validator = null;
        }

        super.destroy();
    }

    protected tabs: JQuery;
    protected toolbar: Toolbar;
    protected validator: JQueryValidation.Validator;
    protected isPanel: boolean;
    protected responsive: boolean;

    public arrange(): void {
        this.element.find('.require-layout').filter(':visible').each(function (i, e) {
            $(e).triggerHandler('layout');
        });
    }

    protected getToolbarButtons(): ToolButton[] {
        return [];
    }

    protected getValidatorOptions(): JQueryValidation.ValidationOptions {
        return {};
    }

    protected initTabs(): void {
        var tabsDiv = this.byId('Tabs');
        if (tabsDiv.length === 0) {
            return;
        }
        this.tabs = (tabsDiv as any).tabs?.({});
    }

    protected initToolbar(): void {
        var toolbarDiv = this.byId('Toolbar');
        if (!toolbarDiv.length)
            return;
        this.toolbar = new Toolbar(toolbarDiv, { buttons: this.getToolbarButtons() });
    }

    protected initValidator(): void {
        var form = this.byId('Form');
        if (form.length > 0) {
            var valOptions = this.getValidatorOptions();
            this.validator = form.validate(validateOptions(valOptions));
        }
    }

    protected resetValidation(): void {
        if (this.validator) {
            (this.validator as any).resetAll();
        }
    }

    protected validateForm(): boolean {
        return this.validator == null || !!this.validator.form();
    }
}

export class TemplatedPanelComponent<P = {}> extends TemplatedPanel<P> {
    constructor(props?: WidgetProps<P>) {
        super(arguments[1], props);
    }

    static override isWidgetComponent: true;
}