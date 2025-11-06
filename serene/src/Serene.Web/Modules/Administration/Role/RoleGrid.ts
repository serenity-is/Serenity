import { EntityGrid } from "@serenity-is/corelib";
import { RoleColumns, RoleRow, RoleService } from "../../ServerTypes/Administration";
import { nsAdministration } from "../../ServerTypes/Namespaces";
import { RoleDialog } from "./RoleDialog";

export class RoleGrid extends EntityGrid<RoleRow, any> {
    static override[Symbol.typeInfo] = this.registerClass(nsAdministration);

    protected override getColumnsKey() { return RoleColumns.columnsKey; }
    protected override getDialogType() { return RoleDialog; }
    protected override getIdProperty() { return RoleRow.idProperty; }
    protected override getLocalTextPrefix() { return RoleRow.localTextPrefix; }
    protected override getService() { return RoleService.baseUrl; }

    constructor(props: any) {
        super(props);
    }

    protected override getDefaultSortBy() {
        return [RoleRow.Fields.RoleName];
    }
}