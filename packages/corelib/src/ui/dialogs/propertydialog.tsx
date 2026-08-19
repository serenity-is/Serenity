import { PropertyItem, PropertyItemsData, cancelDialogButton, getInstanceType, getTypeFullName, nsSerenity, okDialogButton } from "../../base";
import { ScriptData, getFormData, getFormDataAsync } from "../../compat";
import { Attributes, PanelAttribute, StaticPanelAttribute } from "../../types/attributes";
import { PropertyGrid, PropertyGridOptions } from "../widgets/propertygrid";
import { WidgetProps } from "../widgets/widget";
import { BaseDialog } from "./basedialog";

/**
 * A dialog that edits a single entity's properties using a property grid,
 * with OK/Cancel buttons and optional static panel behavior.
 * @typeParam TItem - Entity row type.
 * @typeParam P - Widget props type.
 */
export class PropertyDialog<TItem, P> extends BaseDialog<P> {
    static override[Symbol.typeInfo] = this.registerClass(nsSerenity, [Attributes.panel(false)]);

    declare private _entity: TItem;
    declare private _entityId: any;

    declare protected propertyItemsData: PropertyItemsData;
    /**
     * Whether the dialog can be closed; false for static panels.
     * @returns True when the dialog is closable.
     */
    protected isClosable() { return !this.isStatic(); }
    /**
     * Whether the dialog renders as a static (non-closable) panel.
     * @returns True when static.
     */
    protected isStatic() { return false; }

    /**
     * Creates a property dialog and loads property items.
     * @param props - Widget props forwarded to the base dialog.
     */
    constructor(props?: WidgetProps<P>) {
        super(props);
        this.syncOrAsyncThen(this.getPropertyItemsData, this.getPropertyItemsDataAsync, itemsData => {
            this.propertyItemsReady(itemsData);
            this.afterInit();
        });
    }

    /**
     * Called once property items are available; initializes the property grid and loads the initial entity.
     * @param itemsData - Property items data.
     */
    protected propertyItemsReady(itemsData: PropertyItemsData) {
        this.propertyItemsData = itemsData;
        this.initPropertyGrid();
        this.loadInitialEntity();
    }

    /**
     * Hook invoked after the dialog is initialized.
     */
    protected afterInit() {
    }

    /**
     * Whether property items should be loaded asynchronously.
     * @returns True when async loading is used.
     */
    protected useAsync() {
        return false;
    }

    /**
     * Cleans up the property grid and delegates to the base destroy.
     */
    override destroy() {
        if (this.propertyGrid) {
            this.propertyGrid.destroy();
            this.propertyGrid = null;
        }

        super.destroy();
    }

    /**
     * Returns the dialog options with a narrower width.
     * @returns Dialog options.
     */
    protected override getDialogOptions() {
        var opt = super.getDialogOptions();
        opt.width = 400;
        return opt;
    }

    /**
     * Returns the dialog buttons; static panels have none.
     * @returns Dialog button definitions.
     */
    protected override getDialogButtons() {

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

    /**
     * Handles the OK button click, validating before saving.
     */
    protected okClick() {
        if (!this.validateBeforeSave()) {
            return;
        }

        this.okClickValidated();
    }

    /**
     * Closes the dialog with an OK result after validation passes.
     */
    protected okClickValidated() {
        this.dialogClose("ok");
    }

    /**
     * Closes the dialog with a cancel result.
     */
    protected cancelClick() {
        this.dialogClose("cancel");
    }

    /**
     * Initializes the property grid from the PropertyGrid element.
     */
    protected initPropertyGrid() {
        var pgDiv = this.findById('PropertyGrid');
        if (!pgDiv) {
            return;
        }
        var pgOptions = this.getPropertyGridOptions();
        this.propertyGrid = (new PropertyGrid({ element: pgDiv, ...pgOptions })).init();
    }

    /**
     * Returns the form key derived from the dialog class name.
     * @returns The form key.
     */
    protected getFormKey(): string {
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

    /**
     * Returns the options for the property grid.
     * @returns Property grid options.
     */
    protected getPropertyGridOptions(): PropertyGridOptions {
        return {
            idPrefix: this.idPrefix,
            items: this.getPropertyItems(),
            mode: 1,
            localTextPrefix: 'Forms.' + this.getFormKey() + '.'
        };
    }

    /**
     * Returns the property items for this dialog.
     * @returns The property items.
     */
    protected getPropertyItems(): PropertyItem[] {
        return this.propertyItemsData?.items || [];
    }

    /**
     * Loads the property items data, either from script data or local items.
     * @returns The property items data.
     */
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

    /**
     * Asynchronously loads the property items data.
     * @returns A promise resolving to the property items data.
     */
    protected async getPropertyItemsDataAsync(): Promise<PropertyItemsData> {
        var formKey = this.getFormKey();
        if (formKey) {
            return await getFormDataAsync(formKey);
        }

        return { items: [], additionalItems: [] };
    }

    /**
     * Returns the entity populated from the property grid.
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
     * Loads an empty entity into the property grid.
     */
    protected loadInitialEntity(): void {
        this.propertyGrid && this.propertyGrid.load(new Object());
    }

    /**
     * Returns the current entity.
     * @returns The entity.
     */
    get entity() {
        return this._entity;
    }

    /** Sets the current entity. */
    protected set entity(value: TItem) {
        this._entity = (value ?? new Object()) as any;
    }

    /**
     * Returns the current entity id.
     * @returns The entity id.
     */
    get entityId() {
        return this._entityId;
    }

    /** Sets the current entity id. */
    protected set entityId(value: any) {
        this._entityId = value;
    }

    /**
     * Validates the form before saving.
     * @returns True when the form is valid.
     */
    protected validateBeforeSave(): boolean {
        return this.validator.form();
    }

    /**
     * Hook for subclasses to update the dialog title.
     */
    protected updateTitle() {
    }

    declare protected propertyGrid: PropertyGrid;

    /**
     * Renders the dialog contents with a form and property grid.
     * @returns The rendered content.
     */
    protected override renderContents(): any {
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