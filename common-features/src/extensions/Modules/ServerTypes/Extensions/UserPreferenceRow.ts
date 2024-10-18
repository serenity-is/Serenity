import { fieldsProxy } from "@serenity-is/corelib";

export interface UserPreferenceRow {
    UserPreferenceId?: number;
    UserId?: number;
    PreferenceType?: string;
    Name?: string;
    Value?: string;
}

export abstract class UserPreferenceRow {
    static readonly idProperty = 'UserPreferenceId';
    static readonly nameProperty = 'Name';
    static readonly localTextPrefix = 'Common.UserPreference';
    static readonly deletePermission = '';
    static readonly insertPermission = '';
    static readonly readPermission = '';
    static readonly updatePermission = '';

    static readonly Fields = fieldsProxy<UserPreferenceRow>();
}
