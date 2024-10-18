import { RoleDialog } from "./RoleDialog";
import { RoleColumns, RoleRow, RoleService } from "../";
import { Decorators, EntityGrid } from "@serenity-is/corelib";

@Decorators.registerClass('Serene.Administration.RoleGrid')
export class RoleGrid extends EntityGrid<RoleRow, any> {
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