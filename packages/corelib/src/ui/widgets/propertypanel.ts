import { PropertyItem, getInstanceType, getTypeFullName } from "../../base";
import { getForm } from "../../compat";
import { FormKeyAttribute } from "../../types/attributes";
import { Decorators } from "../../types/decorators";
import { PropertyGrid, PropertyGridMode, PropertyGridOptions } from "./propertygrid";
import { BasePanel } from "./basepanel";
import { WidgetProps } from "./widget";

@Decorators.registerClass('Serenity.PropertyPanel')
export class PropertyPanel<TItem, P> extends BasePanel<P> {

    declare private _entity: TItem;
    declare private _entityId: any;

    constructor(props: WidgetProps<P>) {
        super(props);

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

    protected initPropertyGrid() {
        var pgDiv = this.findById('PropertyGrid');
        if (!pgDiv)
            return;
        var pgOptions = this.getPropertyGridOptions();
        this.propertyGrid = (new PropertyGrid({ element: pgDiv, ...pgOptions })).init();
    }

    protected loadInitialEntity(): void {
        if (this.propertyGrid) {
            this.propertyGrid.load(new Object());
        }
    }

    protected getFormKey(): string {
        var attr = this.getCustomAttribute(FormKeyAttribute);
        if (attr) {
            return attr.value;
        }
        var name = getTypeFullName(getInstanceType(this));
        var px = name.indexOf('.');
        if (px >= 0) {
            name = name.substring(px + 1);
        }
        else if (name.endsWith('Panel')) {
            name = name.substring(0, name.length - 5);
        }
        return name;
    }

    protected getPropertyGridOptions(): PropertyGridOptions {
        return {
            idPrefix: this.idPrefix,
            items: this.getPropertyItems(),
            mode: PropertyGridMode.insert,
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

    public get entity(): TItem {
        return this._entity;
    }

    public get entityId(): any {
        return this._entityId;
    }

    protected set entity(value: TItem) {
        this._entity = value ?? new Object() as any;
    }

    protected set entityId(value: any) {
        this._entityId = value;
    }

    protected validateBeforeSave(): boolean {
        return this.validator.form();
    }

    declare protected propertyGrid: PropertyGrid;
}