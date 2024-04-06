import { PropertyItem, getInstanceType, getTypeFullName } from "../../base";
import { getForm } from "../../q";
import { FormKeyAttribute } from "../../types/attributes";
import { Decorators } from "../../types/decorators";
import { PropertyGrid, PropertyGridOptions } from "./propertygrid";
import { TemplatedPanel } from "./templatedpanel";
import { WidgetProps } from "./widget";

@Decorators.registerClass('Serenity.PropertyPanel')
export class PropertyPanel<TItem, P> extends TemplatedPanel<P> {

    private _entity: TItem;
    private _entityId: any;

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

    protected get_entity(): TItem {
        return this._entity;
    }

    protected get_entityId(): any {
        return this._entityId;
    }
    
    protected set_entity(value: TItem): void {
        this._entity = value ?? new Object() as any;
    }

    protected set_entityId(value: any): void {
        this._entityId = value;
    }

    protected validateBeforeSave(): boolean {
        return this.validator.form();
    }

    protected propertyGrid: PropertyGrid;
}