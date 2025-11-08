import {
    Culture, DataGrid,
    Dictionary, Fluent, GridUtils, Grouping, IGetEditValue, ISetEditValue, SlickFormatting, SlickTreeHelper,
    ToolButton, WidgetProps, count, getRemoteDataAsync, localText, stripDiacritics, toGrouping, tryGetText
} from "@serenity-is/corelib";
import { Column } from "@serenity-is/sleekgrid";
import { UserPermissionRow } from "../../ServerTypes/Administration";
import { nsAdministration } from "../../ServerTypes/Namespaces";
import { RemoteDataKeys } from "../../ServerTypes/RemoteDataKeys";
import { UserPermissionDialogTexts } from "../../ServerTypes/Texts";

export interface PermissionCheckEditorOptions {
    showRevoke?: boolean;
    value?: (UserPermissionRow | string)[];
    rolePermissions?: string[];
    implicitPermissions?: Record<string, string[]>;
}

export interface PermissionCheckItem {
    ParentKey?: string;
    Key?: string;
    Title?: string;
    IsGroup?: boolean;
    GrantRevoke?: boolean;
}

export class PermissionCheckEditor<P extends PermissionCheckEditorOptions = PermissionCheckEditorOptions> extends DataGrid<PermissionCheckItem, P> {
    static override[Symbol.typeInfo] = this.registerEditor(nsAdministration, [IGetEditValue, ISetEditValue]);

    protected override getIdProperty() { return "Key"; }

    declare private searchText: string;
    declare private byParentKey: Grouping<PermissionCheckItem>;

    constructor(props: WidgetProps<P>) {
        super(props);

        let titleByKey: Dictionary<string> = {};
        this.getSortedGroupAndPermissionKeys(titleByKey, (permissionKeys) => {
            if (!this.domNode)
                return;

            let items = permissionKeys.map(key => ({
                Key: key,
                ParentKey: this.getParentKey(key),
                Title: titleByKey[key],
                GrantRevoke: null,
                IsGroup: key.charAt(key.length - 1) === ':'
            } satisfies PermissionCheckItem));

            this.byParentKey = toGrouping(items, x => x.ParentKey);
            this.setItems(items);
            this.value = this._value;
        });

        if (this.options.value) {
            if (typeof this.options.value[0] === "string")
                this.valueAsStrings = this.options.value as string[];
            else
                this.value = this.options.value as UserPermissionRow[];
        }

        this.implicitPermissions = props.implicitPermissions;
        this.rolePermissions = props.rolePermissions;
    }

    private getItemGrantRevokeClass(item: PermissionCheckItem, grant: boolean): string {
        if (!item.IsGroup)
            return ((item.GrantRevoke === grant) ? ' checked' : '');

        const desc = this.getDescendants(item, true);
        const granted = desc.filter(x => x.GrantRevoke === grant);
        return !granted.length ? '' : ((desc.length === granted.length) ? 'checked' : 'partial');
    }

    private hasByRoleOrImplicitly(permission: string): boolean {
        return this.rolePermSet.has(permission) || Object.entries(this.implicitSets).some(([ifperm, permset]) =>
            permset.has(permission) && (this.rolePermSet.has(ifperm) || this.view.getItemById(ifperm)?.GrantRevoke));
    }

    private getItemEffectiveClass(item: PermissionCheckItem): string {
        if (item.IsGroup) {
            const desc = this.getDescendants(item, true);
            const grantCount = count(desc, x => x.GrantRevoke === true || (x.GrantRevoke == null && this.hasByRoleOrImplicitly(x.Key)));
            return (grantCount === desc.length || desc.length === 0) ? 'allow' : (grantCount === 0 ? 'deny' : 'partial');
        }

        return (item.GrantRevoke === true || (item.GrantRevoke == null && this.hasByRoleOrImplicitly(item.Key))) ? 'allow' : 'deny';
    }

    protected override createColumns(): Column[] {
        let columns: Column[] = [{
            name: UserPermissionDialogTexts.Permission,
            field: 'Title',
            format: SlickFormatting.treeToggle(() => this.view, x => x.Key, ctx => <span class={["effective-permission", this.getItemEffectiveClass(ctx.item)]}>{ctx.value}</span>),
            width: 495,
            sortable: false
        }, {
            name: UserPermissionDialogTexts.Grant,
            field: 'Grant',
            format: ctx => <span class={["check-box grant no-float ", this.getItemGrantRevokeClass(ctx.item, true)]} />,
            width: 65,
            sortable: false,
            headerCssClass: 'align-center',
            cssClass: 'align-center'
        }];

        if (this.options.showRevoke) {
            columns.push({
                name: UserPermissionDialogTexts.Revoke,
                field: 'Revoke',
                format: ctx => <span class={["check-box revoke no-float", this.getItemGrantRevokeClass(ctx.item, false)]} />,
                width: 65,
                sortable: false,
                headerCssClass: 'align-center',
                cssClass: 'align-center'
            });
        }

        return columns;
    }

    public override setItems(items: PermissionCheckItem[]): void {
        SlickTreeHelper.setIndents(items, x => x.Key, x => x.ParentKey, false);
        this.view.setItems(items, true);
    }

    protected override getGridCanLoad() {
        return false;
    }

    protected overrideonViewFilter(item: PermissionCheckItem): boolean {
        return super.onViewFilter(item) && SlickTreeHelper.filterById(item, this.view, x => x.ParentKey) &&
            (!this.searchText || (this.matchContains(item) || item.IsGroup && this.getDescendants(item, false).some(x => this.matchContains(x))));
    }

    private matchContains(item: PermissionCheckItem): boolean {
        return stripDiacritics(item.Title || '').toLowerCase().indexOf(this.searchText) >= 0;
    }

    private getDescendants(item: PermissionCheckItem, excludeGroups: boolean): PermissionCheckItem[] {
        const result: PermissionCheckItem[] = [];
        const stack = [item];
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

    protected override onClick(e: Event, row: number, cell: number): void {
        super.onClick(e, row, cell);

        if (!Fluent.isDefaultPrevented(e))
            SlickTreeHelper.toggleClick(e, row, cell, this.view, (x: any) => x.Key);

        if (Fluent.isDefaultPrevented(e))
            return;

        const target = Fluent(e.target);
        let grant = target.hasClass('grant');

        if (grant || target.hasClass('revoke')) {
            e.preventDefault();

            let item = this.itemAt(row);
            let checkedOrPartial = target.hasClass('checked') || target.hasClass('partial');
            grant = checkedOrPartial ? null : grant !== checkedOrPartial;

            if (item.IsGroup)
                this.getDescendants(item, true).forEach(x => x.GrantRevoke = grant);
            else
                item.GrantRevoke = grant;

            this.sleekGrid.invalidate();
        }
    }

    private getParentKey(key: string): string {
        if (key.charAt(key.length - 1) === ':')
            key = key.substring(0, key.length - 1);

        const idx = key.lastIndexOf(':');
        return idx >= 0 ? key.substring(0, idx + 1) : null;
    }

    protected override getButtons(): ToolButton[] {
        return [];
    }

    protected override createToolbarExtensions(): void {
        super.createToolbarExtensions();
        GridUtils.addQuickSearchInputCustom(this.toolbar.domNode, (_, text) => {
            this.searchText = stripDiacritics(text?.trim() ?? '').toLowerCase();
            this.view.setItems(this.view.getItems(), true);
        });
    }

    private getSortedGroupAndPermissionKeys(titleByKey: Dictionary<string>, then: (result: string[]) => void) {
        getRemoteDataAsync(RemoteDataKeys.Administration.PermissionKeys).then((keys: string[]) => {
            let titleWithGroup = {};
            for (var s of keys.filter(s => s)) {
                if (s.charAt(s.length - 1) == ':') {
                    s = s.substring(0, s.length - 1);
                    if (s.length === 0) {
                        continue;
                    }
                }

                if (titleByKey[s]) {
                    continue;
                }

                titleByKey[s] = localText("Permission." + s, s);
                let parts = s.split(':');
                let group = '';
                let groupTitle = '';
                for (let i = 0; i < parts.length - 1; i++) {
                    group = group + parts[i] + ':';
                    let txt = localText("Permission." + group, parts[i]);
                    titleByKey[group] = txt;
                    groupTitle = groupTitle + titleByKey[group] + ':';
                    titleWithGroup[group] = groupTitle;
                }

                titleWithGroup[s] = groupTitle + titleByKey[s];
            }

            keys = Object.keys(titleByKey);
            keys = keys.sort((x, y) => Culture.stringCompare(titleWithGroup[x], titleWithGroup[y]));
            then(keys);
        });
    }

    declare private _value: UserPermissionRow[];

    get value(): UserPermissionRow[] {
        if (!this.view.getItems().length) // not initialized yet
            return (this._value || []).map(x => ({ PermissionKey: x.PermissionKey, Granted: x.Granted }));

        return this.view.getItems().filter(item => item.GrantRevoke != null && item.Key.charAt(item.Key.length - 1) != ':')
            .map(item => ({ PermissionKey: item.Key, Granted: item.GrantRevoke }));
    }

    set value(value: UserPermissionRow[]) {
        this._value = (value || []);
        this.view.getItems().forEach(x => { x.GrantRevoke = null });
        for (let item of this._value) {
            const r = this.view.getItemById(item.PermissionKey);
            r && (r.GrantRevoke = item.Granted ?? true);
        }
        this.setItems(this.getItems());
    }

    get valueAsStrings() {
        return this.value.map(x => x.PermissionKey);
    }

    set valueAsStrings(value: string[]) {
        this.value = (value || []).map(x => ({ PermissionKey: x }));
    }

    private rolePermSet: Set<string> = new Set();

    get rolePermissions(): string[] {
        return Array.from(this.rolePermSet.values());
    }

    set rolePermissions(value: string[]) {
        this.rolePermSet = new Set();
        value?.forEach(x => this.rolePermSet.add(x));
    }

    private implicitSets: Record<string, Set<string>> = {};

    set implicitPermissions(value: Record<string, string[]>) {
        this.implicitSets = {};
        Object.entries(value || {}).forEach(([ifperm, permarr]) =>
            permarr?.forEach(perm => (this.implicitSets[ifperm] ||= new Set()).add(perm)));
    }
}