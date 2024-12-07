import { PropertyItem, PropertyItemsData, cancelDialogButton, getInstanceType, getTypeFullName, okDialogButton } from "../../base";
import { ScriptData, getFormData, getFormDataAsync } from "../../compat";
import { FormKeyAttribute, StaticPanelAttribute } from "../../types/attributes";
import { Decorators } from "../../types/decorators";
import { PropertyGrid, PropertyGridOptions } from "../widgets/propertygrid";
import { WidgetProps } from "../widgets/widget";
import { BaseDialog } from "./basedialog";

@Decorators.registerClass('Serenity.PropertyDialog')
@Decorators.panel(false)
export class PropertyDialog<TItem, P> extends BaseDialog<P> {
    declare private _entity: TItem;
    declare private _entityId: any;

    declare protected propertyItemsData: PropertyItemsData;
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

    get entity() {
        return this._entity;
    }

    protected set entity(value: TItem) {
        this._entity = (value ?? new Object()) as any;
    }

    get entityId() {
        return this._entityId;
    }

    protected set entityId(value: any) {
        this._entityId = value;
    }

    protected validateBeforeSave(): boolean {
        return this.validator.form();
    }

    protected updateTitle() {
    }

    declare protected propertyGrid: PropertyGrid;

    protected renderContents(): any {
        if (this.legacyTemplateRender())
            return void 0;

        const id = this.useIdPrefix();
        return (
            <div class="s-Form">
                <form id={id.Form} action="">
                    <div id={id.PropertyGrid}></div>
                </form>
            </div>);
    }
}