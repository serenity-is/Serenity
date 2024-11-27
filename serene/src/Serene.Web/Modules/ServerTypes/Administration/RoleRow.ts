import { getLookup, getLookupAsync, fieldsProxy } from "@serenity-is/corelib";

export interface RoleRow {
    RoleId?: number;
    RoleName?: string;
}

export abstract class RoleRow {
    static readonly idProperty = 'RoleId';
    static readonly nameProperty = 'RoleName';
    static readonly localTextPrefix = 'Administration.Role';
    static readonly lookupKey = 'Administration.Role';

    /** @deprecated use getLookupAsync instead */
    static getLookup() { return getLookup<RoleRow>('Administration.Role') }
    static async getLookupAsync() { return getLookupAsync<RoleRow>('Administration.Role') }

    static readonly deletePermission = 'Administration:Security';
    static readonly insertPermission = 'Administration:Security';
    static readonly readPermission = 'Administration:Security';
    static readonly updatePermission = 'Administration:Security';

    static readonly Fields = fieldsProxy<RoleRow>();
}