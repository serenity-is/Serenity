import { getLookup, getLookupAsync, fieldsProxy } from "@serenity-is/corelib";

export interface UserRow {
    UserId?: number;
    Username?: string;
    Source?: string;
    PasswordHash?: string;
    PasswordSalt?: string;
    DisplayName?: string;
    Email?: string;
    UserImage?: string;
    LastDirectoryUpdate?: string;
    IsActive?: number;
    Password?: string;
    PasswordConfirm?: string;
    Roles?: number[];
    InsertUserId?: number;
    InsertDate?: string;
    UpdateUserId?: number;
    UpdateDate?: string;
}

export abstract class UserRow {
    static readonly idProperty = 'UserId';
    static readonly isActiveProperty = 'IsActive';
    static readonly nameProperty = 'Username';
    static readonly localTextPrefix = 'Administration.User';
    static readonly lookupKey = 'Administration.User';

    /** @deprecated use getLookupAsync instead */
    static getLookup() { return getLookup<UserRow>('Administration.User') }
    static async getLookupAsync() { return getLookupAsync<UserRow>('Administration.User') }

    static readonly deletePermission = 'Administration:Security';
    static readonly insertPermission = 'Administration:Security';
    static readonly readPermission = 'Administration:Security';
    static readonly updatePermission = 'Administration:Security';

    static readonly Fields = fieldsProxy<UserRow>();
}