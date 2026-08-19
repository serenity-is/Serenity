
import { bindThis } from "@serenity-is/domwise";
import { Authorization, EntityGridTexts, Fluent, faIcon, getActiveRequests, getInstanceType, getTypeFullName, isPromiseLike, localText, nsSerenity, resolveUrl, stringFormat } from "../../base";
import { HandleRouteEvent, Router, replaceAll, safeCast } from "../../compat";
import { IEditDialog } from "../../interfaces";
import { RemoteViewOptions } from "../../slick";
import { DialogType } from "../../types/dialogtype";
import { DialogTypeRegistry } from "../../types/dialogtyperegistry";
import { EditorUtils } from "../editors/editorutils";
import { SubDialogHelper } from "../helpers/subdialoghelper";
import { ToolButton } from "../widgets/toolbar";
import { Widget, WidgetProps } from "../widgets/widget";
import { ColumnPickerDialog } from "./columnpickerdialog";
import { DataGrid } from "./datagrid";
import { FilterDisplayBar } from "../filtering/filterdisplaybar";
import { FilterDialog } from "../filtering/filterdialog";

/**
 * Base grid for entity-bound data; integrates routing, permissions, dialogs,
 * toolbar buttons, filtering and service integration on top of {@link DataGrid}.
 * Registered via modern `static override [Symbol.typeInfo] = this.registerClass(...)`.
 * @typeParam TItem - Entity row type.
 * @typeParam P - Widget props type.
 */
export class EntityGrid<TItem, P = {}> extends DataGrid<TItem, P> {
    static override[Symbol.typeInfo] = this.registerClass(nsSerenity);

    /**
     * Creates an entity grid and wires the route handler.
     * @param props - Widget props forwarded to {@link DataGrid}.
     */
    constructor(props: WidgetProps<P>) {
        super(props);
        this.domNode.classList.add('route-handler');
        Fluent.on(this.domNode, "handleroute." + this.uniqueName, bindThis(this).handleRoute);
    }

    /**
     * Cleans up the route-fixup handler and delegates to the base destroy.
     */
    override destroy() {
        Fluent.off(document, "." + this.uniqueName + "_routerfix");
        super.destroy();
    }

    /**
     * Handles hash-based routing for new/edit dialog invocations.
     * @param e - Route event containing the hash fragment and navigation metadata.
     */
    protected handleRoute(e: HandleRouteEvent): void {

        let route = Fluent.eventProp(e, "route");
        if (typeof route !== "string")
            return;

        if (route === 'new') {
            e.preventDefault();
            this.addButtonClick();
            return;
        }

        var oldRequests = getActiveRequests();

        var parts = route.split('/');
        if (parts.length === 2 && parts[0] === 'edit') {
            e.preventDefault();
            this.editItem(decodeURIComponent(parts[1]));
        }
        else if (parts.length === 2 && parts[1] === 'new') {
            e.preventDefault();
            this.editItemOfType(parts[0], null);
        }
        else if (parts.length === 3 && parts[1] === 'edit') {
            e.preventDefault();
            this.editItemOfType(parts[0], decodeURIComponent(parts[2]));
        }
        else
            return;

        if (!Fluent.eventProp(e, "isInitial"))
            return;

        Fluent.off(document, "." + this.uniqueName + "_routerfix");

        let evParts: string[] = Fluent.eventProp(e, "parts");
        let evIndex = Fluent.eventProp(e, "index");

        if (getActiveRequests() > oldRequests &&
            evParts != null && evIndex != null && evIndex >= 0 && evIndex < evParts.length - 1 &&
            !evParts[evIndex + 1].startsWith("!") &&
            Fluent.isDefaultPrevented(e)) {
            Fluent.one(document, "ajaxStop." + this.uniqueName + "_routerfix", () => {
                window.location.hash = '#' + evParts.join('/+/');
            });
        }
    }

    /**
     * Indicates whether a pager should be rendered; entity grids use a pager by default.
     * @returns True when paging is enabled.
     */
    protected override usePager(): boolean {
        return true;
    }

    /**
     * Creates standard toolbar extensions such as the include-deleted toggle and quick search.
     */
    protected override createToolbarExtensions(): void {
        this.createIncludeDeletedButton();
        this.createQuickSearchInput();
    }

    /**
     * Returns the initial title for the grid panel.
     * @returns Localized plural display name.
     */
    protected override getInitialTitle(): string {
        return this.getDisplayName();
    }

    /**
     * Resolves the local text prefix for this grid; falls back to the entity type.
     * @returns Local text prefix key.
     */
    protected override getLocalTextPrefix(): string {
        var result = super.getLocalTextPrefix();

        if (result != null ||
            this.getRowDefinition())
            return result;

        return this.getEntityType();
    }

    declare private _entityType: string;

    /**
     * Returns the entity type name derived from the grid class name.
     * @returns Entity type (e.g. "Administration.User").
     */
    protected getEntityType(): string {
        if (this._entityType != null)
            return this._entityType;

        var name = getTypeFullName(getInstanceType(this));

        var px = name.indexOf('.');
        if (px >= 0) {
            name = name.substring(px + 1);
        }

        if (name.endsWith('Grid')) {
            name = name.substring(0, name.length - 4);
        }
        else if (name.endsWith('SubGrid')) {
            name = name.substring(0, name.length - 7);
        }

        this._entityType = name;

        return this._entityType;
    }

    declare private _displayName: string;

    /**
     * Returns the localized plural display name for the entity.
     * @returns Display name for the grid title.
     */
    protected getDisplayName(): string {
        if (this._displayName != null)
            return this._displayName;

        return this._displayName = localText(this.getLocalTextDbPrefix() + 'EntityPlural', this.getEntityType());
    }

    declare private _itemName: string;

    /**
     * Returns the localized singular name for a single entity item.
     * @returns Singular display name.
     */
    protected getItemName(): string {
        if (this._itemName != null)
            return this._itemName;

        return this._itemName = localText(this.getLocalTextDbPrefix() + "EntitySingular", this.getEntityType());
    }

    /**
     * Returns the caption for the add/new button.
     * @returns Localized add button text.
     */
    protected override getAddButtonCaption(): string {
        return stringFormat(EntityGridTexts.NewButton, this.getItemName());
    }

    /**
     * Builds the grid toolbar buttons including add, refresh, column picker and filter bar.
     * @returns Array of tool button definitions.
     */
    protected override getButtons(): ToolButton[] {

        var buttons: ToolButton[] = [];
        buttons.push({
            title: this.getAddButtonCaption(),
            action: 'add',
            cssClass: 'add-button',
            icon: faIcon("plus-circle", "green"),
            hotkey: 'alt+n',
            onClick: () => {
                this.addButtonClick();
            },
            disabled: () => !this.hasInsertPermission() || this.readOnly
        });

        buttons.push(this.newRefreshButton(true));
        buttons.push(ColumnPickerDialog.createToolButton(this as any));
        buttons.push(FilterDisplayBar.createToolButton({
            onClick: () => {
                if (!this.filterBar)
                    return;
                const dialog = new FilterDialog({});
                dialog.get_filterPanel().set_store(this.filterBar.get_store());
                dialog.dialogOpen(null);
            },
            visible: () => this.filterBar != null
        }));

        return buttons;
    }

    /**
     * Shows or hides the filter bar depending on whether active filters exist.
     */
    protected setFilterBarVisibility(): void {
        if (!this.filterBar.domNode)
            return;

        if (this.filterBar.domNode.hidden != !this.filterBar?.get_store()?.get_items()?.length) {
            Fluent.toggle(this.filterBar.domNode);
            this.layout();
        }
    }

    /**
     * Creates the filter bar and syncs its visibility.
     */
    protected override createFilterBar(): void {
        super.createFilterBar();
        this.setFilterBarVisibility();
    }

    /**
     * Invoked when the filter store changes; refreshes the filter bar visibility.
     */
    protected override filterStoreChanged() {
        super.filterStoreChanged();
        this.setFilterBarVisibility();
    }

    /**
     * Creates a refresh toolbar button.
     * @param noText - When true, only an icon with a hint is shown.
     * @returns Tool button definition for refreshing the grid.
     */
    protected newRefreshButton(noText?: boolean): ToolButton {
        return {
            title: (noText ? null : EntityGridTexts.RefreshButton),
            hint: (noText ? EntityGridTexts.RefreshButton : null),
            icon: faIcon("refresh", "blue"),
            action: 'refresh',
            cssClass: 'refresh-button',
            onClick: () => {
                this.refresh();
            }
        };
    }

    /**
     * Handles the add button click by opening a new-item dialog.
     */
    protected addButtonClick(): void {
        this.editItem(new Object());
    }

    /**
     * Opens an edit dialog for an existing entity or a new instance.
     * @param entityOrId - Entity instance or identifier to edit.
     */
    protected override editItem(entityOrId: any): void {
        this.createEntityDialog(this.getItemType(), dlg => {
            var dialog = safeCast(dlg, IEditDialog);
            if (dialog != null) {
                dialog.load(entityOrId, () => {
                    dialog.dialogOpen(this.openDialogsAsPanel ?? DataGrid.defaultOptions.openDialogsAsPanel);
                });

                return;
            }

            throw new Error(
                stringFormat("{0} doesn't implement IEditDialog!",
                    getTypeFullName(getInstanceType(dlg))));
        });
    }

    /**
     * Opens an edit dialog for a specific item type, used for polymorphic entities.
     * @param itemType - Entity type key.
     * @param entityOrId - Entity instance or identifier to edit.
     */
    protected override editItemOfType(itemType: string, entityOrId: any): void {

        if (itemType === this.getItemType()) {
            this.editItem(entityOrId);
            return;
        }

        this.createEntityDialog(itemType, dlg => {
            var dialog = safeCast(dlg, IEditDialog);
            if (dialog != null) {
                dialog.load(entityOrId, () => {
                    dialog.dialogOpen(this.openDialogsAsPanel ?? DataGrid.defaultOptions.openDialogsAsPanel);
                });
                return;
            }

            throw new Error(
                stringFormat("{0} doesn't implement IEditDialog!",
                    getTypeFullName(getInstanceType(dlg))));
        });
    }

    declare private _service: string;

    /**
     * Returns the service endpoint path for the entity (e.g. "Administration/User").
     * @returns Service path derived from the entity type.
     */
    protected getService(): string {
        if (this._service != null)
            return this._service;

        return this._service = replaceAll(this.getEntityType(), '.', '/');
    }

    /**
     * Returns the service method name for listing entities.
     * @returns Service method (defaults to "<service>/List").
     */
    protected getServiceMethod() {
        return this.getService() + '/List';
    }

    /**
     * Returns the absolute URL for the list service endpoint.
     * @returns Resolved service URL.
     */
    protected getServiceUrl() {
        return resolveUrl('~/Services/' + this.getServiceMethod());
    }

    /**
     * Returns view options including the service URL.
     * @returns Remote view options with URL populated.
     */
    protected override getViewOptions(): RemoteViewOptions {
        var opt = super.getViewOptions();
        opt.url = this.getServiceUrl();
        return opt;
    }

    /**
     * Returns the entity item type key used to resolve dialogs and row definitions.
     * @returns Item type key.
     */
    protected override getItemType() {
        return this.getEntityType();
    }

    /**
     * Registers hash-based routing for the specified dialog.
     * @param itemType - Entity type key for the dialog.
     * @param dialog - Dialog widget to route.
     */
    protected routeDialog(itemType: string, dialog: Widget<any>) {
        Router && Router.dialog && Router.dialog(this.domNode, dialog.domNode, () => {
            var hash = '';

            if (itemType !== this.getItemType())
                hash = itemType + '/';

            if (dialog != null && (dialog as any).entityId != null)
                hash += 'edit/' + (dialog as any).entityId.toString();
            else
                hash += 'new';

            return hash;
        });
    }
    
    /**
     * Returns the permission key required to insert a row.
     * @returns Insert permission or undefined if none is configured.
     */
    protected getInsertPermission(): string {
        return this.getRowDefinition()?.insertPermission;
    }

    /**
     * Returns the permission key required to update a row.
     * @returns Update permission or undefined.
     */
    protected getUpdatePermission(): string {
        return this.getRowDefinition()?.updatePermission;
    }

    /**
     * Returns the permission key required to delete a row.
     * @returns Delete permission or undefined.
     */
    protected getDeletePermission(): string {
        return this.getRowDefinition()?.deletePermission;
    }

    /**
     * Checks whether the current user may delete rows.
     * @returns True when deletion is allowed.
     */
    protected hasDeletePermission() {
        const deletePermission = this.getDeletePermission();
        return deletePermission == null || Authorization.hasPermission(deletePermission);
    }

    /**
     * Checks whether the current user may insert rows.
     * @returns True when insertion is allowed.
     */
    protected hasInsertPermission() {
        const insertPermission = this.getInsertPermission();
        return insertPermission == null || Authorization.hasPermission(insertPermission);
    }

    /**
     * Checks whether the current user may update rows.
     * @returns True when updates are allowed.
     */
    protected hasUpdatePermission() {
        const updatePermission = this.getUpdatePermission();
        return updatePermission == null || Authorization.hasPermission(updatePermission);
    }

    /**
     * Propagates the grid read-only state to a newly created dialog.
     * @param dialog - Dialog instance to configure.
     */
    protected transferDialogReadOnly(dialog: Widget<any>) {
        if (this.readOnly)
            EditorUtils.setReadOnly(dialog, true);
    }

    /**
     * Initializes a dialog bound to the primary item type.
     * @param dialog - Dialog instance to initialize.
     */
    protected initDialog(dialog: Widget<any>): void {
        SubDialogHelper.bindToDataChange(dialog, this, (_) => {
            this.subDialogDataChange();
        }, true);

        this.transferDialogReadOnly(dialog);
        this.routeDialog(this.getItemType(), dialog);
    }

    /**
     * Initializes a dialog for the given item type, wiring data-change and routing.
     * @param itemType - Entity type key.
     * @param dialog - Dialog instance to initialize.
     */
    protected initEntityDialog(itemType: string, dialog: Widget<any>): void {
        if (itemType === this.getItemType()) {
            this.initDialog(dialog);
            return;
        }

        SubDialogHelper.bindToDataChange(dialog, this, (_) => {
            this.subDialogDataChange();
        }, true);

        this.transferDialogReadOnly(dialog);
        this.routeDialog(itemType, dialog);
    }

    /**
     * Creates a dialog widget for the specified item type, initializing and routing it.
     * @param itemType - Entity type key whose dialog type will be resolved.
     * @param callback - Optional callback invoked with the created dialog.
     * @returns The dialog instance or a promise that resolves to it.
     */
    protected createEntityDialog(itemType: string, callback?: (dlg: Widget<any>) => void): (Widget<any> | PromiseLike<Widget<any>>) {
        const dialogType = this.getDialogTypeFor(itemType);

        const then = (dialogType: any) => {
            var dialog = Widget.create({
                type: dialogType,
                options: this.getDialogOptionsFor(itemType)
            });
            this.initEntityDialog(itemType, dialog);
            callback?.(dialog);
            return dialog;
        }

        if (isPromiseLike(dialogType)) {
            return dialogType.then(then);
        }
        else {
            return then(dialogType);
        }
    }

    /**
     * Returns default options for the primary entity dialog.
     * @returns Dialog options.
     */
    protected getDialogOptions(): any {
        return {};
    }

    /**
     * Returns dialog options for a specific item type.
     * @param itemType - Entity type key.
     * @returns Dialog options for that type.
     */
    protected getDialogOptionsFor(itemType: string): any {
        if (itemType === this.getItemType())
            return this.getDialogOptions();

        return {};
    }

    /**
     * Resolves the dialog type for the given item type.
     * @param itemType - Entity type key.
     * @returns Dialog constructor or a promise that resolves to it.
     */
    protected getDialogTypeFor(itemType: string): DialogType | PromiseLike<DialogType> {

        if (itemType === this.getItemType()) {
            return this.getDialogType();
        }

        return DialogTypeRegistry.getOrLoad(itemType);
    }

    declare private _dialogType: any;

    /**
     * Resolves the dialog type for the primary entity; cached and may be loaded lazily.
     * @returns Dialog constructor or a promise that resolves to it.
     */
    protected getDialogType(): DialogType | PromiseLike<DialogType> {

        if (this._dialogType != null)
            return this._dialogType;

        const promise = DialogTypeRegistry.getOrLoad(this.getEntityType());
        if (isPromiseLike(promise)) {
            return promise.then(t => {
                this._dialogType = t;
                return t;
            });
        }

        this._dialogType = promise;
        return this._dialogType;
    }
}