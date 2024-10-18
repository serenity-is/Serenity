import { proxyTexts } from "@serenity-is/corelib";

namespace texts {

    export declare namespace Db {

        namespace Common {

            namespace UserPreference {
                export const Name: string;
                export const PreferenceType: string;
                export const UserId: string;
                export const UserPreferenceId: string;
                export const Value: string;
            }
        }
    }

    export declare namespace Forms {

        namespace Membership {

            namespace ChangePassword {
                export const FormTitle: string;
                export const SubmitButton: string;
                export const Success: string;
            }

            namespace ForgotPassword {
                export const FormInfo: string;
                export const FormTitle: string;
                export const SubmitButton: string;
                export const SuccessMessage: string;
            }

            namespace ResetPassword {
                export const EmailSubject: string;
                export const FormTitle: string;
                export const SubmitButton: string;
                export const Success: string;
            }

            namespace SetPassword {
                export const ElevatedActionsMessage: string;
                export const EmailSentMessage: string;
                export const EmailToSetPasswordMessage: string;
                export const PageTitle: string;
                export const SendEmailButton: string;
            }
        }
    }

    export declare namespace Site {

        namespace BasicProgressDialog {
            export const CancelTitle: string;
            export const PleaseWait: string;
        }

        namespace BulkServiceAction {
            export const AllHadErrorsFormat: string;
            export const AllSuccessFormat: string;
            export const ConfirmationFormat: string;
            export const ErrorCount: string;
            export const NothingToProcess: string;
            export const SomeHadErrorsFormat: string;
            export const SuccessCount: string;
        }

        namespace Dialogs {
            export const PendingChangesConfirmation: string;
            export const PendingChangesUnloadWarning: string;
        }

        namespace Translation {
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
        export const InvalidResetToken: string;
        export const MinRequiredPasswordLength: string;
        export const PasswordConfirmMismatch: string;
        export const PasswordStrengthRequireDigit: string;
        export const PasswordStrengthRequireLowercase: string;
        export const PasswordStrengthRequireNonAlphanumeric: string;
        export const PasswordStrengthRequireUppercase: string;
    }

}

export const Texts: typeof texts = proxyTexts({}, '', {
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

export const DialogUtilsTexts = Texts.Site.Dialogs;

export const ExtensionsTexts = Texts;

export const ForgotPasswordFormTexts = Texts.Forms.Membership.ForgotPassword;

export const PasswordStrengthValidationTexts = Texts.Validation;

export const ResetPasswordFormTexts = Texts.Forms.Membership.ResetPassword;

export const SetPasswordFormTexts = Texts.Forms.Membership.SetPassword;

export const TranslationTexts = Texts.Site.Translation;