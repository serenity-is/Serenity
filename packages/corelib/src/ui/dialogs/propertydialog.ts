import { PropertyItem, PropertyItemsData, cancelDialogButton, getInstanceType, getTypeFullName, okDialogButton } from "../../base";
import { ScriptData, getFormData, getFormDataAsync } from "../../q";
import { FormKeyAttribute, StaticPanelAttribute } from "../../types/attributes";
import { Decorators } from "../../types/decorators";
import { PropertyGrid, PropertyGridOptions } from "../widgets/propertygrid";
import { WidgetProps } from "../widgets/widget";
import { TemplatedDialog } from "./templateddialog";

@Decorators.registerClass('Serenity.PropertyDialog')
@Decorators.panel(false)
export class PropertyDialog<TItem, P> extends TemplatedDialog<P> {
    protected entity: TItem;
    protected entityId: any;
    protected propertyItemsData: PropertyItemsData;
    protected isClosable() { return !this.isStatic(); }
    protected isStatic() { return false; }

    constructor(props?: WidgetProps<P>) {
        super(props);
        this.syncOrAsyncThen(this.getPropertyItemsData, this.getPropertyItemsDataAsync, itemsData => {
            this.propertyItemsReady(itemsData);
            this.afterInit();
        });
    }

    protected propertyItemsReady(itemsData: PropertyItemsData) {
        this.propertyItemsData = itemsData;
        this.initPropertyGrid();
        this.loadInitialEntity();
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

        if (this.getCustomAttribute(StaticPanelAttribute)?.value === true)
            return [];

        return [
            okDialogButton({
                click: (e) => {
                    e.preventDefault();
                    this.okClick();
                }
            }),
            cancelDialogButton()
        ];
    }

    protected okClick() {
        if (!this.validateBeforeSave()) {
            return;
        }

        this.okClickValidated();
    }

    protected okClickValidated() {
        this.dialogClose("ok");
    }

    protected cancelClick() {
        this.dialogClose("cancel");
    }

    protected initPropertyGrid() {
        var pgDiv = this.findById('PropertyGrid');
        if (!pgDiv) {
            return;
        }
        var pgOptions = this.getPropertyGridOptions();
        this.propertyGrid = (new PropertyGrid({ element: pgDiv, ...pgOptions })).init();
    }

    protected getFormKey(): string {
        var attr = this.getCustomAttribute(FormKeyAttribute);
        if (attr) {
            return attr.value;
        }
        else {
            var name = getTypeFullName(getInstanceType(this));
            var px = name.indexOf('.');
            if (px >= 0) {
                name = name.substring(px + 1);
            }
            if (name.endsWith('Dialog')) {
                name = name.substring(0, name.length - 6);
            }
            else if (name.endsWith('Panel')) {
                name = name.substring(0, name.length - 5);
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

        if (formKey) {
            return getFormData(formKey);
        }

        return { items: [], additionalItems: [] };
    }

    protected async getPropertyItemsDataAsync(): Promise<PropertyItemsData> {
        var formKey = this.getFormKey();
        if (formKey) {
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

    protected getTemplate() {
        return `<div class="s-Form"><form id="~_Form" action=""><div id="~_PropertyGrid"></div></form></div>`;
    }
}