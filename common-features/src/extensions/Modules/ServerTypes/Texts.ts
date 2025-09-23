import { proxyTexts } from "@serenity-is/corelib";

namespace texts {
    export declare namespace Db {
        export function asKey(): typeof Db;
        export function asTry(): typeof Db;
        namespace Common {
            export function asKey(): typeof Common;
            export function asTry(): typeof Common;
            namespace UserPreference {
                export function asKey(): typeof UserPreference;
                export function asTry(): typeof UserPreference;
                export const EntityPlural: string;
                export const EntitySingular: string;
                export const Name: string;
                export const PreferenceType: string;
                export const UserId: string;
                export const UserPreferenceId: string;
                export const Value: string;
            }
        }
    }
    export declare namespace Forms {
        export function asKey(): typeof Forms;
        export function asTry(): typeof Forms;
        namespace Membership {
            export function asKey(): typeof Membership;
            export function asTry(): typeof Membership;
            namespace ChangePassword {
                export function asKey(): typeof ChangePassword;
                export function asTry(): typeof ChangePassword;
                export const FormTitle: string;
                export const SubmitButton: string;
                export const Success: string;
            }
            namespace ForgotPassword {
                export function asKey(): typeof ForgotPassword;
                export function asTry(): typeof ForgotPassword;
                export const FormInfo: string;
                export const FormTitle: string;
                export const SubmitButton: string;
                export const SuccessMessage: string;
            }
            namespace ResetPassword {
                export function asKey(): typeof ResetPassword;
                export function asTry(): typeof ResetPassword;
                export const EmailSubject: string;
                export const FormTitle: string;
                export const SubmitButton: string;
                export const Success: string;
            }
            namespace SetPassword {
                export function asKey(): typeof SetPassword;
                export function asTry(): typeof SetPassword;
                export const ElevatedActionsMessage: string;
                export const EmailSentMessage: string;
                export const EmailToSetPasswordMessage: string;
                export const PageTitle: string;
                export const SendEmailButton: string;
            }
        }
    }
    export declare namespace Site {
        export function asKey(): typeof Site;
        export function asTry(): typeof Site;
        namespace BasicProgressDialog {
            export function asKey(): typeof BasicProgressDialog;
            export function asTry(): typeof BasicProgressDialog;
            export const CancelTitle: string;
            export const PleaseWait: string;
        }
        namespace BulkServiceAction {
            export function asKey(): typeof BulkServiceAction;
            export function asTry(): typeof BulkServiceAction;
            export const AllHadErrorsFormat: string;
            export const AllSuccessFormat: string;
            export const ConfirmationFormat: string;
            export const ErrorCount: string;
            export const NothingToProcess: string;
            export const SomeHadErrorsFormat: string;
            export const SuccessCount: string;
        }
        namespace Dialogs {
            export function asKey(): typeof Dialogs;
            export function asTry(): typeof Dialogs;
            export const PendingChangesConfirmation: string;
            export const PendingChangesUnloadWarning: string;
        }
        namespace Translation {
            export function asKey(): typeof Translation;
            export function asTry(): typeof Translation;
            export const AllTextsAlreadyTranslated: string;
            export const Assembly: string;
            export const CopyFailMessage: string;
            export const CopySourceTranslations: string;
            export const CopySuccessMessage: string;
            export const CopyTargetTranslations: string;
            export const CustomText: string;
            export const EntityPlural: string;
            export const HasTranslation: string;
            export const Key: string;
            export const OverrideConfirmation: string;
            export const SaveChangesButton: string;
            export const SaveSuccessMessage: string;
            export const SourceLanguage: string;
            export const SourceTargetLanguageSame: string;
            export const SourceText: string;
            export const TargetLanguage: string;
            export const TargetLanguageRequired: string;
            export const TargetText: string;
            export const TranslateAllText: string;
            export const TranslateText: string;
            export const TranslateTextConfirmation: string;
            export const TranslateTextDisabled: string;
            export const UserTranslated: string;
        }
    }
    export declare namespace Validation {
        export function asKey(): typeof Validation;
        export function asTry(): typeof Validation;
        export const InvalidResetToken: string;
        export const MinRequiredPasswordLength: string;
        export const PasswordConfirmMismatch: string;
        export const PasswordStrengthRequireDigit: string;
        export const PasswordStrengthRequireLowercase: string;
        export const PasswordStrengthRequireNonAlphanumeric: string;
        export const PasswordStrengthRequireUppercase: string;
    }

}

const Texts: typeof texts = proxyTexts({}, '', {
    Db: {
        Common: {
            UserPreference: {}
        }
    },
    Forms: {
        Membership: {
            ChangePassword: {},
            ForgotPassword: {},
            ResetPassword: {},
            SetPassword: {}
        }
    },
    Site: {
        BasicProgressDialog: {},
        BulkServiceAction: {},
        Dialogs: {},
        Translation: {}
    },
    Validation: {}
}) as any;

export const BasicProgressDialogTexts = Texts.Site.BasicProgressDialog;
export const BulkServiceActionTexts = Texts.Site.BulkServiceAction;
export const ChangePasswordFormTexts = Texts.Forms.Membership.ChangePassword;
export const ChangePasswordValidationTexts = Texts.Validation;
export const DialogUtilsTexts = Texts.Site.Dialogs;
export const ExtensionsTexts = Texts;
export const ForgotPasswordFormTexts = Texts.Forms.Membership.ForgotPassword;
export const PasswordStrengthValidationTexts = Texts.Validation;
export const ResetPasswordFormTexts = Texts.Forms.Membership.ResetPassword;
export const SetPasswordFormTexts = Texts.Forms.Membership.SetPassword;
export const TranslationTexts = Texts.Site.Translation;