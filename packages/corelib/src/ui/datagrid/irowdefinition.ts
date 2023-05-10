export interface IRowDefinition {
    readonly deletePermission?: string;
    readonly idProperty?: string;
    readonly insertPermission?: string;
    readonly isActiveProperty?: string;
    readonly isDeletedProperty?: string;
    readonly localTextPrefix?: string;
    readonly nameProperty?: string;   
    readonly readPermission?: string;
    readonly updatePermission?: string;
}