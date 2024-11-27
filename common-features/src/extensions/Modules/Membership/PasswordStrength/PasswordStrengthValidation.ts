import { PasswordEditor, getRemoteDataAsync, localText, stringFormat } from "@serenity-is/corelib";
import { PasswordStrengthRules } from "../../ServerTypes/Extensions/PasswordStrengthRules";

export async function getPasswordStrengthRules(): Promise<PasswordStrengthRules> {
    return await getRemoteDataAsync<PasswordStrengthRules>("PasswordStrengthRules");
}

export function addPasswordStrengthValidation(passwordEditor: PasswordEditor, uniqueName?: string) {
    getPasswordStrengthRules().then((passwordStrengthRules) => {
        passwordEditor.addValidationRule(() => {
            if (passwordEditor.value.length < passwordStrengthRules.MinPasswordLength)
                return stringFormat(localText("Validation.MinRequiredPasswordLength"), passwordStrengthRules.MinPasswordLength);
        }, uniqueName);

        if (passwordStrengthRules.RequireDigit) {
            passwordEditor.addValidationRule(() => {
                if (!(/[0-9]/.test(passwordEditor.value)))
                    return stringFormat(localText("Validation.PasswordStrengthRequireDigit"));
            }, uniqueName);
        }        

        if (passwordStrengthRules.RequireLowercase) {
            passwordEditor.addValidationRule(() => {
                if (!(/[a-z\p{Ll}]/u.test(passwordEditor.value)))
                    return localText("Validation.PasswordStrengthRequireLowercase");
            }, uniqueName);
        }

        if (passwordStrengthRules.RequireUppercase) {
            passwordEditor.addValidationRule(() => {
                if (!(/[A-Z\p{Lu}]/u.test(passwordEditor.value)))
                    return localText("Validation.PasswordStrengthRequireUppercase");
            }, uniqueName);
        }

        if (passwordStrengthRules.RequireNonAlphanumeric) {
            passwordEditor.addValidationRule(() => {
                if (!(/[^\s\p{L}]/u.test(passwordEditor.value)))
                    return localText("Validation.PasswordStrengthRequireNonAlphanumeric");
            }, uniqueName);
        }
    });
}