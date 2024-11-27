import { fieldsProxy } from "@serenity-is/corelib";

export interface RolePermissionRow {
    RolePermissionId?: number;
    RoleId?: number;
    PermissionKey?: string;
    RoleName?: string;
}

export abstract class RolePermissionRow {
    static readonly idProperty = 'RolePermissionId';
    static readonly nameProperty = 'PermissionKey';
    static readonly localTextPrefix = 'Administration.RolePermission';
    static readonly deletePermission = 'Administration:Security';
    static readonly insertPermission = 'Administration:Security';
    static readonly readPermission = 'Administration:Security';
    static readonly updatePermission = 'Administration:Security';

    static readonly Fields = fieldsProxy<RolePermissionRow>();
}