import { registerClass } from "../../Decorators";
import { IEditDialog } from "../../Interfaces/IEditDialog";
import { Authorization } from "../../Q/Authorization";
import { format } from "../../Q/Formatting";
import { LT, text, tryGetText } from "../../Q/LocalText";
import { Router } from "../../Q/Router";
import { resolveUrl } from "../../Q/Services";
import { endsWith, isEmptyOrNull, replaceAll } from "../../Q/Strings";
import { cast, getInstanceType, getTypeFullName, safeCast } from "../../Q/TypeSystem";
import { DialogTypeAttribute, DisplayNameAttribute, EntityTypeAttribute, ItemNameAttribute, ServiceAttribute } from "../../Types/Attributes";
import { DialogTypeRegistry } from "../../Types/DialogTypeRegistry";
import { EditorUtils } from "../Editors/EditorUtils";
import { SubDialogHelper } from "../Helpers/SubDialogHelper";
import { ToolButton } from "../Widgets/Toolbar";
import { Widget, WidgetDialogClass } from "../Widgets/Widget";
import { ColumnPickerDialog } from "./ColumnPickerDialog";
import { DataGrid } from "./DataGrid";

@registerClass('Serenity.EntityGrid')
export class EntityGrid<TItem, TOptions> extends DataGrid<TItem, TOptions> {

    constructor(container: JQuery, options?: TOptions) {
        super(container, options);

        this.element.addClass('route-handler')
            .on('handleroute.' + this.uniqueName, (e: JQueryEventObject, arg: any) => {
                if (!!arg.handled)
                    return;

                if (!!(arg.route === 'new')) {
                    arg.handled = true;
                    this.addButtonClick();
                    return;
                }

                var parts = arg.route.split('/');
                if (!!(parts.length === 2 && parts[0] === 'edit')) {
                    arg.handled = true;
                    this.editItem(parts[1]);
                    return;
                }

                if (!!(parts.length === 2 && parts[1] === 'new')) {
                    arg.handled = true;
                    this.editItemOfType(cast(parts[0], String), null);
                    return;
                }

                if (!!(parts.length === 3 && parts[1] === 'edit')) {
                    arg.handled = true;
                    this.editItemOfType(cast(parts[0], String), parts[2]);
                }
            });
    }

    protected usePager(): boolean {
        return true;
    }

    protected createToolbarExtensions(): void {
        this.createIncludeDeletedButton();
        this.createQuickSearchInput();
    }

    protected getInitialTitle(): string {
        return this.getDisplayName();
    }

    protected getLocalTextPrefix(): string {
        var result = super.getLocalTextPrefix();

        if (isEmptyOrNull(result)) {
            return this.getEntityType();
        }

        return result;
    }

    private entityType: string;

    protected getEntityType(): string {
        if (this.entityType != null)
            return this.entityType;

        var attr = this.attrs(EntityTypeAttribute);

        if (attr.length === 1) {
            return (this.entityType = attr[0].value);
        }

        var name = getTypeFullName(getInstanceType(this));

        var px = name.indexOf('.');
        if (px >= 0) {
            name = name.substring(px + 1);
        }

        if (endsWith(name, 'Grid')) {
            name = name.substr(0, name.length - 4);
        }
        else if (endsWith(name, 'SubGrid')) {
            name = name.substr(0, name.length - 7);
        }

        this.entityType = name;

        return this.entityType;
    }

    private displayName: string;

    protected getDisplayName(): string {
        if (this.displayName != null)
            return this.displayName;

        var attr = this.attrs(DisplayNameAttribute);
        if (attr.length >= 1) {
            this.displayName = attr[0].displayName;
            this.displayName = LT.getDefault(this.displayName, this.displayName);
        }
        else {
            this.displayName = tryGetText(this.getLocalTextDbPrefix() + 'EntityPlural');
            if (this.displayName == null)
                this.displayName = this.getEntityType();
        }

        return this.displayName;
    }

    private itemName: string;

    protected getItemName(): string {
        if (this.itemName != null)
            return this.itemName;

        var attr = this.attrs(ItemNameAttribute);
        if (attr.length >= 1) {
            this.itemName = attr[0].value;
            this.itemName = LT.getDefault(this.itemName, this.itemName);
        }
        else {
            this.itemName = tryGetText(this.getLocalTextDbPrefix() + 'EntitySingular');
            if (this.itemName == null)
                this.itemName = this.getEntityType();
        }

        return this.itemName;
    }

    protected getAddButtonCaption(): string {
        return format(text('Controls.EntityGrid.NewButton'), this.getItemName());
    }

    protected getButtons(): ToolButton[] {

        var buttons: ToolButton[] = [];
        buttons.push({
            title: this.getAddButtonCaption(),
            cssClass: 'add-button',
            hotkey: 'alt+n',
            onClick: () => {
                this.addButtonClick();
            },
            disabled: () => !this.hasInsertPermission() || this.readOnly
            });

        buttons.push(this.newRefreshButton(true));
        buttons.push(ColumnPickerDialog.createToolButton(this as any));

        return buttons;
    }

    protected newRefreshButton(noText?: boolean): ToolButton {
        return {
            title: (noText ? null : text('Controls.EntityGrid.RefreshButton')),
            hint: (noText ? text('Controls.EntityGrid.RefreshButton') : null),
            cssClass: 'refresh-button',
            onClick: () => {
                this.refresh();
            }
        };
    }

    protected addButtonClick(): void {
        this.editItem(new Object());
    }

    protected editItem(entityOrId: any): void {
        this.createEntityDialog(this.getItemType(), dlg => {
            var dialog = safeCast(dlg, IEditDialog);
            if (dialog != null) {
                dialog.load(entityOrId, () => {
                    dialog.dialogOpen(this.openDialogsAsPanel);
                });

                return;
            }

            throw new Error(
                format("{0} doesn't implement IEditDialog!",
                    getTypeFullName(getInstanceType(dlg))));
        });
    }

    protected editItemOfType(itemType: string, entityOrId: any): void {

        if (itemType === this.getItemType()) {
            this.editItem(entityOrId);
            return;
        }

        this.createEntityDialog(itemType, dlg => {
            var dialog = safeCast(dlg, IEditDialog);
            if (dialog != null) {
                dialog.load(entityOrId, () =>
                    dialog.dialogOpen(this.openDialogsAsPanel));
                return;
            }

            throw new Error(
                format("{0} doesn't implement IEditDialog!",
                    getTypeFullName(getInstanceType(dlg))));
        });
    }

    private service: string;

    protected getService(): string {
        if (this.service != null)
            return this.service;

        var attr = this.attrs(ServiceAttribute);
        if (attr.length >= 1)
            this.service = attr[0].value;
        else
            this.service = replaceAll(this.getEntityType(), '.', '/');

        return this.service;
    }

    protected getViewOptions(): Slick.RemoteViewOptions {
        var opt = super.getViewOptions();
        opt.url = resolveUrl('~/Services/' + this.getService() + '/List');
        return opt;
    }

    protected getItemType() {
        return this.getEntityType();
    }

    protected routeDialog(itemType: string, dialog: Widget<any>) {
        Router && Router.dialog && Router.dialog(this.element, dialog.element, () => {
            var hash = '';

            if (itemType !== this.getItemType())
                hash = itemType + '/';

            if (!!(dialog != null && (dialog as any).entityId != null))
                hash += 'edit/' + (dialog as any).entityId.toString();
            else
                hash += 'new';

            return hash;
        });
    }

    protected getInsertPermission(): string {
        return null;
    }

    protected hasInsertPermission(): boolean {
        var insertPermission = this.getInsertPermission();
        return insertPermission == null || Authorization.hasPermission(this.getInsertPermission());
    }

    protected transferDialogReadOnly(dialog: Widget<any>) {
        if (this.readOnly)
            EditorUtils.setReadOnly(dialog, true);
    }

    protected initDialog(dialog: Widget<any>): void {
        SubDialogHelper.bindToDataChange(dialog, this, (e, dci) => {
            this.subDialogDataChange();
        }, true);

        this.transferDialogReadOnly(dialog);
        this.routeDialog(this.getItemType(), dialog);
    }

    protected initEntityDialog(itemType: string, dialog: Widget<any>): void {
        if (itemType === this.getItemType()) {
            this.initDialog(dialog);
            return;
        }

        SubDialogHelper.bindToDataChange(dialog, this, (e, dci) => {
            this.subDialogDataChange();
        }, true);

        this.transferDialogReadOnly(dialog);
        this.routeDialog(itemType, dialog);
    }

    protected createEntityDialog(itemType: string, callback?: (dlg: Widget<any>) => void): Widget<any> {
        var dialogClass = this.getDialogTypeFor(itemType);
        var dialog = Widget.create({
            type: dialogClass,
            options: this.getDialogOptionsFor(itemType),
            init: d => {
                this.initEntityDialog(itemType, d);
                callback && callback(d);
            }
        });

        return dialog;
    }

    protected getDialogOptions(): JQueryUI.DialogOptions {
        return {};
    }

    protected getDialogOptionsFor(itemType: string): JQueryUI.DialogOptions {
        if (itemType === this.getItemType())
            return this.getDialogOptions();

        return {};
    }

    protected getDialogTypeFor(itemType: string): { new(...args: any[]): Widget<any> } {

        if (itemType === this.getItemType()) {
            return this.getDialogType();
        }

        return DialogTypeRegistry.get(itemType) as any;
    }

    private dialogType: WidgetDialogClass;

    protected getDialogType(): { new(...args: any[]): Widget<any> } {

        if (this.dialogType != null)
            return this.dialogType;

        var attr = this.attrs(DialogTypeAttribute);
        if (attr.length >= 1)
            this.dialogType = attr[0].value;
        else
            this.dialogType = DialogTypeRegistry.get(this.getEntityType());

        return this.dialogType;
    }
}
