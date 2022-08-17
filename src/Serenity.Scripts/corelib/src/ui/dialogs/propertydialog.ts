import { Decorators, FormKeyAttribute } from "../../decorators";
import { DialogButton, endsWith, getAttributes, getForm, getInstanceType, getTypeFullName, PropertyItem, text } from "../../q";
import { PropertyGrid, PropertyGridOptions } from "../widgets/propertygrid";
import { TemplatedDialog } from "./templateddialog";

@Decorators.registerClass('Serenity.PropertyDialog')
export class PropertyDialog<TItem, TOptions> extends TemplatedDialog<TOptions> {
    protected _entity: TItem;
    protected _entityId: any;

    constructor(opt?: TOptions) {
        super(opt);

        this.initPropertyGrid();
        this.loadInitialEntity();
    }

    destroy() {
        if (this.propertyGrid) {
            this.propertyGrid.destroy();
            this.propertyGrid = null;
        }

        if (this.validator) {
            this.byId('Form').remove();
            this.validator = null;
        }

        super.destroy();
    }

    protected getDialogOptions() {
        var opt = super.getDialogOptions();
        opt.width = 400;
        return opt;
    }

    protected getDialogButtons() {
        super.getDialogButtons();
        return <DialogButton[]>[{
            text: text('Dialogs.OkButton'),
            cssClass: "btn btn-primary",
            click: () => this.okClick()
        }, {
            text: text('Dialogs.CancelButton'),
            click: () => this.cancelClick()
        }];
    }

    protected okClick() {
        if (!this.validateBeforeSave()) {
            return;
        }

        this.okClickValidated();
    }

    protected okClickValidated() {
        this.dialogClose();
    }

    protected cancelClick() {
        this.dialogClose();
    }

    protected initPropertyGrid() {
        var pgDiv = this.byId('PropertyGrid');
        if (pgDiv.length <= 0) {
            return;
        }
        var pgOptions = this.getPropertyGridOptions();
        this.propertyGrid = (new PropertyGrid(pgDiv, pgOptions)).init(null);
        if (this.element.closest('.ui-dialog').hasClass('s-Flexify')) {
            this.propertyGrid.element.children('.categories').flexHeightOnly(1);
        }
    }

    protected getFormKey(): string {
        var attributes = getAttributes(
            getInstanceType(this), FormKeyAttribute, true);
        if (attributes.length >= 1) {
            return attributes[0].value;
        }
        else {
            var name = getTypeFullName(getInstanceType(this));
            var px = name.indexOf('.');
            if (px >= 0) {
                name = name.substring(px + 1);
            }
            if (endsWith(name, 'Dialog')) {
                name = name.substr(0, name.length - 6);
            }
            else if (endsWith(name, 'Panel')) {
                name = name.substr(0, name.length - 5);
            }
            return name;
        }
    }

    protected getPropertyGridOptions(): PropertyGridOptions {
        return {
            idPrefix: this.idPrefix,
            items: this.getPropertyItems(),
            mode: 1,
            useCategories: false,
            localTextPrefix: 'Forms.' + this.getFormKey() + '.'
        };
    }

    protected getPropertyItems(): PropertyItem[] {
        var formKey = this.getFormKey();
        return getForm(formKey);
    }

    protected getSaveEntity(): TItem {
        var entity = new Object();
        if (this.propertyGrid) {
            this.propertyGrid.save(entity);
        }
        return entity as TItem;
    }

    protected loadInitialEntity(): void {
        this.propertyGrid && this.propertyGrid.load(new Object());
    }

    protected get_entity() {
        return this._entity;
    }

    protected set_entity(value: TItem) {
        this._entity = (value ?? new Object()) as any;
    }

    protected get_entityId() {
        return this._entityId;
    }

    protected set_entityId(value: any): void {
        this._entityId = value;
    }

    protected validateBeforeSave(): boolean {
        return this.validator.form();
    }

    protected updateTitle() {
    }

    protected propertyGrid: PropertyGrid;

    protected getFallbackTemplate() {
        return `<div>
    <div class="s-Form">
        <form id="~_Form" action="">
            <div class="fieldset">
                <div id="~_PropertyGrid"></div>
                <div class="clear"></div>
            </div>
        </form> 
    </div>
</div>`;
    }
}