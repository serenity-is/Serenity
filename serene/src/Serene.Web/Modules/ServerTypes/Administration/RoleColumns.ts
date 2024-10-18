import { ColumnsBase, fieldsProxy } from "@serenity-is/corelib";
import { Column } from "@serenity-is/sleekgrid";
import { RoleRow } from "./RoleRow";

export interface RoleColumns {
    RoleId: Column<RoleRow>;
    RoleName: Column<RoleRow>;
}

export class RoleColumns extends ColumnsBase<RoleRow> {
    static readonly columnsKey = 'Administration.Role';
    static readonly Fields = fieldsProxy<RoleColumns>();
}