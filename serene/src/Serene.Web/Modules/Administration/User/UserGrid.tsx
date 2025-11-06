import { EntityGrid, Lookup, tryFirst } from "@serenity-is/corelib";
import { RoleRow, UserColumns, UserRow, UserService } from "../../ServerTypes/Administration";
import { nsAdministration } from "../../ServerTypes/Namespaces";
import { UserDialog } from "./UserDialog";

export class UserGrid extends EntityGrid<UserRow, any> {
    static override[Symbol.typeInfo] = this.registerClass(nsAdministration);

    protected override getColumnsKey() { return UserColumns.columnsKey; }
    protected override getDialogType() { return UserDialog; }
    protected override getIdProperty() { return UserRow.idProperty; }
    protected override getIsActiveProperty() { return UserRow.isActiveProperty; }
    protected override getLocalTextPrefix() { return UserRow.localTextPrefix; }
    protected override getService() { return UserService.baseUrl; }

    constructor(props: any) {
        super(props);
    }

    protected override getDefaultSortBy() {
        return [UserRow.Fields.Username];
    }

    protected override createIncludeDeletedButton() {
    }

    protected override createColumns() {
        var columns = super.createColumns();

        var roles = tryFirst(columns, x => x.field == UserRow.Fields.Roles);
        if (roles) {
            var rolesLookup: Lookup<RoleRow>;
            RoleRow.getLookupAsync().then(lookup => {
                rolesLookup = lookup;
                this.sleekGrid.invalidate();
            });

            roles.format = ctx => {
                if (!rolesLookup)
                    return <i class="fa fa-spinner"></i>;

                var roleList = (ctx.value || []).map(x => (rolesLookup.itemById[x] || {}).RoleName || "");
                roleList.sort();
                return roleList.map(x => ctx.escape(x)).join(", ");
            };
        }

        return columns;
    }
}
