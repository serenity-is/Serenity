import { fieldsProxy } from "@serenity-is/corelib";

export interface UserPermissionRow {
    UserPermissionId?: number;
    UserId?: number;
    PermissionKey?: string;
    Granted?: boolean;
    Username?: string;
    User?: string;
}

export abstract class UserPermissionRow {
    static readonly idProperty = 'UserPermissionId';
    static readonly nameProperty = 'PermissionKey';
    static readonly localTextPrefix = 'Administration.UserPermission';
    static readonly deletePermission = 'Administration:Security';
    static readonly insertPermission = 'Administration:Security';
    static readonly readPermission = 'Administration:Security';
    static readonly updatePermission = 'Administration:Security';

    static readonly Fields = fieldsProxy<UserPermissionRow>();
}