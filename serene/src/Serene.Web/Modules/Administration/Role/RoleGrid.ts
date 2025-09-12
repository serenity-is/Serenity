import { EntityGrid } from "@serenity-is/corelib";
import { RoleColumns, RoleRow, RoleService } from "../../ServerTypes/Administration";
import { RoleDialog } from "./RoleDialog";

export class RoleGrid extends EntityGrid<RoleRow, any> {
    static override typeInfo = this.registerClass("Serene.Administration.RoleGrid");

    protected getColumnsKey() { return RoleColumns.columnsKey; }
    protected getDialogType() { return RoleDialog; }
    protected getIdProperty() { return RoleRow.idProperty; }
    protected getLocalTextPrefix() { return RoleRow.localTextPrefix; }
    protected getService() { return RoleService.baseUrl; }

    constructor(props: any) {
        super(props);
    }

    protected getDefaultSortBy() {
        return [RoleRow.Fields.RoleName];
    }
}