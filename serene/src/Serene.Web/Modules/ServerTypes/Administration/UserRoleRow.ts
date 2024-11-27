import { fieldsProxy } from "@serenity-is/corelib";

export interface UserRoleRow {
    UserRoleId?: number;
    UserId?: number;
    RoleId?: number;
    Username?: string;
    User?: string;
    RoleName?: string;
}

export abstract class UserRoleRow {
    static readonly idProperty = 'UserRoleId';
    static readonly localTextPrefix = 'Administration.UserRole';
    static readonly deletePermission = 'Administration:Security';
    static readonly insertPermission = 'Administration:Security';
    static readonly readPermission = 'Administration:Security';
    static readonly updatePermission = 'Administration:Security';

    static readonly Fields = fieldsProxy<UserRoleRow>();
}