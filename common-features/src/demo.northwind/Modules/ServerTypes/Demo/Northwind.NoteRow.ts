import { fieldsProxy } from "@serenity-is/corelib";

export interface NoteRow {
    NoteId?: number;
    EntityType?: string;
    EntityId?: number;
    Text?: string;
    InsertUserId?: number;
    InsertDate?: string;
    InsertUserDisplayName?: string;
}

export abstract class NoteRow {
    static readonly idProperty = 'NoteId';
    static readonly nameProperty = 'EntityType';
    static readonly localTextPrefix = 'Northwind.Note';
    static readonly deletePermission = 'Northwind:General';
    static readonly insertPermission = 'Northwind:General';
    static readonly readPermission = 'Northwind:General';
    static readonly updatePermission = 'Northwind:General';

    static readonly Fields = fieldsProxy<NoteRow>();
}