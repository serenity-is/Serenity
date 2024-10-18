import { DataGrid, Decorators, Dictionary, Fluent, GridUtils, Grouping, IGetEditValue, ISetEditValue, SlickFormatting, SlickTreeHelper, ToolButton, WidgetProps, any, count, getRemoteData, htmlEncode, localText, stripDiacritics, toGrouping, trimToNull, tryGetText, turkishLocaleCompare } from "@serenity-is/corelib";
import { Column } from "@serenity-is/sleekgrid";
import { UserPermissionRow } from "../";

@Decorators.registerEditor('Serene.Administration.PermissionCheckEditor', [IGetEditValue, ISetEditValue])
export class PermissionCheckEditor extends DataGrid<PermissionCheckItem, PermissionCheckEditorOptions> {

    protected getIdProperty() { return "Key"; }

    private searchText: string;
    private byParentKey: Grouping<PermissionCheckItem>;

    constructor(props: WidgetProps<PermissionCheckEditorOptions>) {
        super(props);

        let titleByKey: Dictionary<string> = {};
        let permissionKeys = this.getSortedGroupAndPermissionKeys(titleByKey);
        let items = permissionKeys.map(key => <PermissionCheckItem>{
            Key: key,
            ParentKey: this.getParentKey(key),
            Title: titleByKey[key],
            GrantRevoke: null,
            IsGroup: key.charAt(key.length - 1) === ':'
        });

        this.byParentKey = toGrouping(items, x => x.ParentKey);
        this.setItems(items);
    }

    private getItemGrantRevokeClass(item: PermissionCheckItem, grant: boolean): string {
        if (!item.IsGroup) {
            return ((item.GrantRevoke === grant) ? ' checked' : '');
        }

        let desc = this.getDescendants(item, true);
        let granted = desc.filter(x => x.GrantRevoke === grant);

        if (!granted.length) {
            return '';
        }

        if (desc.length === granted.length) {
            return 'checked';
        }

        return 'checked partial';
    }

    private roleOrImplicit(key): boolean {
        if (this._rolePermissions[key])
            return true;

        for (var k of Object.keys(this._rolePermissions)) {
            var d = this._implicitPermissions[k];
            if (d && d[key])
                return true;
        }

        for (var i of Object.keys(this._implicitPermissions)) {
            var item = this.view.getItemById(i);
            if (item && item.GrantRevoke == true) {
                var d = this._implicitPermissions[i];
                if (d && d[key])
                    return true;
            }
        }
    }

    private getItemEffectiveClass(item: PermissionCheckItem): string {

        if (item.IsGroup) {
            let desc = this.getDescendants(item, true);
            let grantCount = count(desc, x => x.GrantRevoke === true ||
                (x.GrantRevoke == null && this.roleOrImplicit(x.Key)));

            if (grantCount === desc.length || desc.length === 0) {
                return 'allow';
            }

            if (grantCount === 0) {
                return 'deny';
            }

            return 'partial';
        }

        let granted = item.GrantRevoke === true ||
            (item.GrantRevoke == null && this.roleOrImplicit(item.Key));

        return (granted ? ' allow' : ' deny');
    }

    protected getColumns(): Column[] {
        let columns: Column[] = [{
            name: localText('Site.UserPermissionDialog.Permission'),
            field: 'Title',
            format: SlickFormatting.treeToggle(() => this.view, x => x.Key, ctx => {
                let item = ctx.item;
                let klass = this.getItemEffectiveClass(item);
                return '<span class="effective-permission ' + klass + '">' + htmlEncode(ctx.value) + '</span>';
            }),
            width: 495,
            sortable: false
        }, {
            name: localText('Site.UserPermissionDialog.Grant'), field: 'Grant',
            format: ctx => {
                let item1 = ctx.item;
                let klass1 = this.getItemGrantRevokeClass(item1, true);
                return "<span class='check-box grant no-float " + klass1 + "'></span>";
            },
            width: 65,
            sortable: false,
            headerCssClass: 'align-center',
            cssClass: 'align-center'
        }];

        if (this.options.showRevoke) {
            columns.push({
                name: localText('Site.UserPermissionDialog.Revoke'), field: 'Revoke',
                format: ctx => {
                    let item2 = ctx.item;
                    let klass2 = this.getItemGrantRevokeClass(item2, false);
                    return '<span class="check-box revoke no-float ' + klass2 + '"></span>';
                },
                width: 65,
                sortable: false,
                headerCssClass: 'align-center',
                cssClass: 'align-center'
            });
        }

        return columns;
    }

    public setItems(items: PermissionCheckItem[]): void {
        SlickTreeHelper.setIndents(items, x => x.Key, x => x.ParentKey, false);
        this.view.setItems(items, true);
    }

    protected onViewSubmit() {
        return false;
    }

    protected onViewFilter(item: PermissionCheckItem): boolean {
        if (!super.onViewFilter(item)) {
            return false;
        }

        if (!SlickTreeHelper.filterById(item, this.view, x => x.ParentKey))
            return false;

        if (this.searchText) {
            return this.matchContains(item) || item.IsGroup && any(this.getDescendants(item, false), x => this.matchContains(x));
        }

        return true;
    }

    private matchContains(item: PermissionCheckItem): boolean {
        return stripDiacritics(item.Title || '').toLowerCase().indexOf(this.searchText) >= 0 ||
            stripDiacritics(item.Key || '').toLowerCase().indexOf(this.searchText) >= 0;
    }

    private getDescendants(item: PermissionCheckItem, excludeGroups: boolean): PermissionCheckItem[] {
        let result: PermissionCheckItem[] = [];
        let stack = [item];
        while (stack.length > 0) {
            let i = stack.pop();
            let children = this.byParentKey[i.Key];
            if (!children)
                continue;

            for (let child of children) {
                if (!excludeGroups || !child.IsGroup) {
                    result.push(child);
                }

                stack.push(child);
            }
        }

        return result;
    }

    protected onClick(e: MouseEvent, row, cell): void {
        super.onClick(e, row, cell);

        if (!e.defaultPrevented && !(e as any).isDefaultPrevented?.()) {
            SlickTreeHelper.toggleClick(e, row, cell, this.view, x => x.Key);
        }

        if (e.defaultPrevented || (e as any).isDefaultPrevented?.()) {
            return;
        }

        let target = Fluent(e.target);
        let grant = target.hasClass('grant');

        if (grant || target.hasClass('revoke')) {
            e.preventDefault();

            let item = this.itemAt(row);
            let checkedOrPartial = target.hasClass('checked') || target.hasClass('partial');

            if (checkedOrPartial) {
                grant = null;
            }
            else {
                grant = grant !== checkedOrPartial;
            }

            if (item.IsGroup) {
                for (var d of this.getDescendants(item, true)) {
                    d.GrantRevoke = grant;
                }
            }
            else
                item.GrantRevoke = grant;

            this.slickGrid.invalidate();
        }
    }

    private getParentKey(key): string {
        if (key.charAt(key.length - 1) === ':') {
            key = key.substr(0, key.length - 1);
        }

        let idx = key.lastIndexOf(':');
        if (idx >= 0) {
            return key.substr(0, idx + 1);
        }
        return null;
    }

    protected getButtons(): ToolButton[] {
        return [];
    }

    protected createToolbarExtensions(): void {
        super.createToolbarExtensions();
        GridUtils.addQuickSearchInputCustom(this.toolbar.element, (_, text) => {
            this.searchText = stripDiacritics(trimToNull(text) || '').toLowerCase();
            this.view.setItems(this.view.getItems(), true);
        });
    }

    private getSortedGroupAndPermissionKeys(titleByKey: Dictionary<string>): string[] {
        let keys = <string[]>getRemoteData('Administration.PermissionKeys');
        let titleWithGroup = {};
        for (var k of keys) {
            let s = k;

            if (!s) {
                continue;
            }

            if (s.charAt(s.length - 1) == ':') {
                s = s.substring(0, s.length - 1);
                if (s.length === 0) {
                    continue;
                }
            }

            if (titleByKey[s]) {
                continue;
            }

            titleByKey[s] = tryGetText('Permission.' + s) ?? s;
            let parts = s.split(':');
            let group = '';
            let groupTitle = '';
            for (let i = 0; i < parts.length - 1; i++) {
                group = group + parts[i] + ':';
                let txt = tryGetText('Permission.' + group);
                if (txt == null) {
                    txt = parts[i];
                }
                titleByKey[group] = txt;
                groupTitle = groupTitle + titleByKey[group] + ':';
                titleWithGroup[group] = groupTitle;
            }

            titleWithGroup[s] = groupTitle + titleByKey[s];
        }

        keys = Object.keys(titleByKey);
        keys = keys.sort((x, y) => turkishLocaleCompare(titleWithGroup[x], titleWithGroup[y]));

        return keys;
    }

    get value(): UserPermissionRow[] {

        let result: UserPermissionRow[] = [];

        for (let item of this.view.getItems()) {
            if (item.GrantRevoke != null && item.Key.charAt(item.Key.length - 1) != ':') {
                result.push({ PermissionKey: item.Key, Granted: item.GrantRevoke });
            }
        }

        return result;
    }

    set value(value: UserPermissionRow[]) {

        for (let item of this.view.getItems()) {
            item.GrantRevoke = null;
        }

        if (value != null) {
            for (let row of value) {
                let r = this.view.getItemById(row.PermissionKey);
                if (r) {
                    r.GrantRevoke = row.Granted ?? true;
                }
            }
        }

        this.setItems(this.getItems());
    }

    private _rolePermissions: Dictionary<boolean> = {};

    get rolePermissions(): string[] {
        return Object.keys(this._rolePermissions);
    }

    set rolePermissions(value: string[]) {
        this._rolePermissions = {};

        if (value) {
            for (let k of value) {
                this._rolePermissions[k] = true;
            }
        }

        this.setItems(this.getItems());
    }

    private _implicitPermissions: Dictionary<Dictionary<boolean>> = {};

    set implicitPermissions(value: Dictionary<string[]>) {
        this._implicitPermissions = {};

        if (value) {
            for (var k of Object.keys(value)) {
                this._implicitPermissions[k] = this._implicitPermissions[k] || {};
                var l = value[k];
                if (l) {
                    for (var s of l)
                        this._implicitPermissions[k][s] = true;
                }
            }
        }
    }
}

export interface PermissionCheckEditorOptions {
    showRevoke?: boolean;
}

export interface PermissionCheckItem {
    ParentKey?: string;
    Key?: string;
    Title?: string;
    IsGroup?: boolean;
    GrantRevoke?: boolean;
}
