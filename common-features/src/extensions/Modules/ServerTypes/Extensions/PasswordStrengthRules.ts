export interface PasswordStrengthRules {
    MinPasswordLength?: number;
    RequireDigit?: boolean;
    RequireLowercase?: boolean;
    RequireNonAlphanumeric?: boolean;
    RequireUppercase?: boolean;
}