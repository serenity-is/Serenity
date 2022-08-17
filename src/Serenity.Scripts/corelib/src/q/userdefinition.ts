export interface UserDefinition {
    Username?: string;
    DisplayName?: string;
    IsAdmin?: boolean;
    Permissions?: { [key: string]: boolean };
}