import { Dialog, DialogTexts, PropertyDialog, Widget, WidgetProps, cancelDialogButton, getTypeFullName, getWidgetFrom, okDialogButton, sanitizeHtml, toggleClass, type DialogOptions, type RenderableContent } from "@serenity-is/corelib";
import { nsExtensions } from "../ServerTypes/Namespaces";

export interface PromptDialogOptions {
    cssClass?: string;
    editorType?: string | { new(props?: any): Widget };
    editorOptions?: any;
    title?: string;
    message?: RenderableContent;
    closeOnEscape?: boolean;
    /** @deprecated, set message as HTML element */
    isHtml?: boolean;
    value?: any;
    required?: boolean;
    submitOnEnter?: boolean;
    validateValue: (v: any) => boolean;
}

export class PromptDialog<P extends PromptDialogOptions = PromptDialogOptions> extends PropertyDialog<any, P> {
    static override[Symbol.typeInfo] = this.registerClass(nsExtensions);

    static readonly defaultOptions: Partial<PromptDialogOptions> = {
        required: true,
        submitOnEnter: true,
        closeOnEscape: true,
    };    

    constructor(props: WidgetProps<P>) {
        super({ 
            ...PromptDialog.defaultOptions, 
            ...(props || {} as any) 
        });

        if (this.options.cssClass)
            toggleClass(this.domNode, this.options.cssClass);

        if (this.options.message) {
            var msg = document.createElement("div");
            msg.classList.add("message");
            this.byId("PropertyGrid").prepend(msg);
            if ((this.options as any).isHtml &&
                typeof this.options.message === "string")
                msg.innerHTML = sanitizeHtml(this.options.message);
            else
                msg.append(this.options.message ?? "");
        }

        this.dialogTitle = this.options.title ?? DialogTexts.PromptTitle;
        if (this.options.submitOnEnter ?? true) {
            this.byId("Value").on("keydown", (e) => {
                if (e.key === "Enter") {
                    e.preventDefault();
                    this.dialog?.getFooterNode()?.querySelector('button')?.click();
                }
            });
        }
    }

    protected override getDialogOptions(): DialogOptions {
        return {
            ...super.getDialogOptions(),
            closeOnEscape: this.options.closeOnEscape,
        };
    }

    protected override getDialogButtons() {
        return [
            okDialogButton({
                click: (e: Event) => {
                    if (!this.validateForm()) {
                        e.preventDefault();
                        return;
                    }

                    if (this.options.validateValue != null && !this.options.validateValue(this.value)) {
                        e.preventDefault();
                    }
                },
            }),
            cancelDialogButton()
        ];
    }

    protected override loadInitialEntity() {
        this.value = this.options.value;
    }

    protected override getPropertyItems() {
        return [
            {
                name: "Value",
                editorType: typeof this.options.editorType === "function" ? getTypeFullName(this.options.editorType) : this.options.editorType || "String",
                required: !!this.options.required,
                editorParams: this.options.editorOptions
            }
        ]
    }

    public getEditor<T extends Widget>(widgetType: { new(props?: any): T }): T {
        return getWidgetFrom(this.byId("Value"), widgetType);
    }

    public get value() {
        return (this.getSaveEntity() as any).Value;
    }

    public set value(v: any) {
        this.propertyGrid.load(
            {
                Value: v
            });
    }

    public static prompt(title: string, message: string, value: string, validateValue: (string) => boolean) {
        new PromptDialog({
            title: title,
            message: message,
            value: value,
            validateValue: v => validateValue(v)
        }).dialogOpen();
    }
}