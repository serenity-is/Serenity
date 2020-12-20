import { registerClass } from "../../Decorators";
import { getForm } from "../../Q/ScriptData";
import { endsWith } from "../../Q/Strings";
import { getAttributes, getInstanceType, getTypeFullName } from "../../Q/TypeSystem";
import { FormKeyAttribute } from "../../Types/Attributes";
import { PropertyGrid, PropertyGridOptions } from "./PropertyGrid";
import { TemplatedPanel } from "./TemplatedPanel";

@registerClass('Serenity.PropertyPanel')
export class PropertyPanel<TItem, TOptions> extends TemplatedPanel<TOptions> {

    private _entity: TItem;
    private _entityId: any;

    constructor(container: JQuery, options?: TOptions) {
        super(container, options);

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
        var pgDiv = this.byId('PropertyGrid');
        if (pgDiv.length <= 0) {
            return;
        }
        var pgOptions = this.getPropertyGridOptions();
        this.propertyGrid = (new PropertyGrid(pgDiv, pgOptions)).init(null);
        if (this.element.closest('.ui-Panel').hasClass('s-Flexify')) {
            this.propertyGrid.element.children('.categories').flexHeightOnly(1);
        }
    }

    protected loadInitialEntity(): void {
        if (this.propertyGrid) {
            this.propertyGrid.load(new Object());
        }
    }

    protected getFormKey(): string {
        var attributes = getAttributes(
            getInstanceType(this), FormKeyAttribute, true);

        if (attributes.length >= 1) {
            return attributes[0].value;
        }
        var name = getTypeFullName(getInstanceType(this));
        var px = name.indexOf('.');
        if (px >= 0) {
            name = name.substring(px + 1);
        }
        if (endsWith(name, 'Panel')) {
            name = name.substr(0, name.length - 6);
        }
        else if (endsWith(name, 'Panel')) {
            name = name.substr(0, name.length - 5);
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

    protected getPropertyItems(): Serenity.PropertyItem[] {
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