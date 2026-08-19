import { PropertyItem, getInstanceType, getTypeFullName, nsSerenity } from "../../base";
import { getForm } from "../../compat";
import { BasePanel } from "./basepanel";
import { PropertyGrid, PropertyGridMode, PropertyGridOptions } from "./propertygrid";
import { WidgetProps } from "./widget";

/**
 * A panel that hosts a {@link PropertyGrid} for editing an entity, providing
 * load/save of the entity and its id, and deriving form options from the
 * panel's type name.
 * @typeParam TItem - The entity type edited by the panel.
 * @typeParam P - Widget props type.
 */
export class PropertyPanel<TItem, P> extends BasePanel<P> {
    static override[Symbol.typeInfo] = this.registerClass(nsSerenity);

    declare private _entity: TItem;
    declare private _entityId: any;

    /**
     * Creates a property panel, initializing the property grid and loading the
     * initial (empty) entity.
     * @param props - Widget props forwarded to {@link BasePanel}.
     */
    constructor(props: WidgetProps<P>) {
        super(props);

        this.initPropertyGrid();
        this.loadInitialEntity();
    }

    /**
     * Destroys the property grid and validator, then delegates to the base destroy.
     */
    override destroy() {
        if (this.propertyGrid) {
            this.propertyGrid.destroy();
            this.propertyGrid = null;
        }
        if (this.validator) {
            this.validator?.destroy?.();
            this.byId('Form').remove();
            this.validator = null;
        }
        super.destroy();
    }

    /**
     * Initializes the property grid from the `PropertyGrid` div, if present.
     */
    protected initPropertyGrid() {
        var pgDiv = this.findById('PropertyGrid');
        if (!pgDiv)
            return;
        var pgOptions = this.getPropertyGridOptions();
        this.propertyGrid = (new PropertyGrid({ element: pgDiv, ...pgOptions })).init();
    }

    /**
     * Loads an empty entity into the property grid.
     */
    protected loadInitialEntity(): void {
        if (this.propertyGrid) {
            this.propertyGrid.load(new Object());
        }
    }

    /**
     * Returns the form key derived from the panel's type name, used to look up
     * the form definition and local text prefix.
     * @returns The form key (e.g. "MyPanel" for "MyModule.MyPanel").
     */
    protected getFormKey(): string {
        var name = getTypeFullName(getInstanceType(this));
        var px = name.indexOf('.');
        if (px >= 0) {
            name = name.substring(px + 1);
        }

        if (name.endsWith('Panel'))
            name = name.substring(0, name.length - 5);

        return name;
    }

    /**
     * Returns the options used to configure the property grid.
     * @returns The property grid options.
     */
    protected getPropertyGridOptions(): PropertyGridOptions {
        return {
            idPrefix: this.idPrefix,
            items: this.getPropertyItems(),
            mode: PropertyGridMode.insert,
            localTextPrefix: 'Forms.' + this.getFormKey() + '.'
        };
    }

    /**
     * Returns the property items for the panel's form.
     * @returns The property items to render.
     */
    protected getPropertyItems(): PropertyItem[] {
        var formKey = this.getFormKey();
        return getForm(formKey);
    }

    /**
     * Saves the current editor values into a new entity object.
     * @returns The saved entity.
     */
    protected getSaveEntity(): TItem {
        var entity = new Object();
        if (this.propertyGrid) {
            this.propertyGrid.save(entity);
        }
        return entity as TItem;
    }

    /**
     * Gets the entity currently loaded in the panel.
     */
    public get entity(): TItem {
        return this._entity;
    }

    /**
     * Gets the id of the entity currently loaded in the panel.
     */
    public get entityId(): any {
        return this._entityId;
    }

    /**
     * Sets the entity loaded in the panel.
     * @param value - The entity to set; null is replaced with an empty object.
     */
    protected set entity(value: TItem) {
        this._entity = value ?? new Object() as any;
    }

    /**
     * Sets the id of the entity loaded in the panel.
     * @param value - The entity id to set.
     */
    protected set entityId(value: any) {
        this._entityId = value;
    }

    /**
     * Validates the form before saving.
     * @returns True if the form is valid.
     */
    protected validateBeforeSave(): boolean {
        return this.validator.form();
    }

    /** The property grid hosted by this panel. */
    declare protected propertyGrid: PropertyGrid;
}