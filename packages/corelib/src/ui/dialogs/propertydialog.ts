import { Decorators, FormKeyAttribute } from "../../decorators";
import { DialogButton, endsWith, getAttributes, getForm, getFormData, getFormDataAsync, getInstanceType, getTypeFullName, isEmptyOrNull, PropertyItem, PropertyItemsData, ScriptData, localText } from "@serenity-is/corelib/q";
import { PropertyGrid, PropertyGridOptions } from "../widgets/propertygrid";
import { TemplatedDialog } from "./templateddialog";

@Decorators.registerClass('Serenity.PropertyDialog')
export class PropertyDialog<TItem, TOptions> extends TemplatedDialog<TOptions> {
    protected entity: TItem;
    protected entityId: any;
    protected propertyItemsData: PropertyItemsData;

    constructor(opt?: TOptions) {
        super(opt);

        if (this.useAsync())
            this.initAsync();
        else 
            this.initSync();        
    }

    internalInit() {
        this.initPropertyGrid();
        this.loadInitialEntity();
    }

    protected initSync() {
        this.propertyItemsData = this.getPropertyItemsData();
        this.internalInit();
        this.afterInit();
    }

    protected async initAsync() {
        this.propertyItemsData = await this.getPropertyItemsDataAsync();
        this.internalInit();
        this.afterInit();
    }

    protected afterInit() {
    }    

    protected useAsync() {
        return false;
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
            text: localText('Dialogs.OkButton'),
            cssClass: "btn btn-primary",
            click: () => this.okClick()
        }, {
            text: localText('Dialogs.CancelButton'),
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

    protected getPropertyItems() {
        return this.propertyItemsData?.items || [];
    }

    protected getPropertyItemsData(): PropertyItemsData {
        var formKey = this.getFormKey();

        if (this.getFormKey === PropertyDialog.prototype.getFormKey &&
            this.getPropertyItems !== PropertyDialog.prototype.getPropertyItems &&
            !ScriptData.canLoad('Form.' + formKey)) {
            return {
                items: this.getPropertyItems(),
                additionalItems: []
            }
        }

        if (!isEmptyOrNull(formKey)) {
            return getFormData(formKey);
        }

        return { items: [], additionalItems: [] };
    }

    protected async getPropertyItemsDataAsync(): Promise<PropertyItemsData> {
        var formKey = this.getFormKey();
        if (!isEmptyOrNull(formKey)) {
            return await getFormDataAsync(formKey);
        }

        return { items: [], additionalItems: [] };
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
        return this.entity;
    }

    protected set_entity(value: TItem) {
        this.entity = (value ?? new Object()) as any;
    }

    protected get_entityId() {
        return this.entityId;
    }

    protected set_entityId(value: any): void {
        this.entityId = value;
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