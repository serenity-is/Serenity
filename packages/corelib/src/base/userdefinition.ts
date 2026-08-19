/**
 * Describes the currently authenticated user as resolved on the client.
 * Typically populated from the server's user definition script.
 */
export interface UserDefinition {
    /** Username / login name of the current user. */
    Username?: string;
    /** Human-readable display name of the current user. */
    DisplayName?: string;
    /**
     * Whether the user is a super-admin with implicit access to all permissions.
     * This is distinct from membership in an Administrators role, which may not
     * grant every permission individually.
     */
    IsAdmin?: boolean;
    /**
     * Map of permission keys granted to the user (explicitly or via roles).
     * Client-side checks should only drive UI enable/disable; always re-validate
     * permissions on the server.
     */
    Permissions?: { [key: string]: boolean };
}