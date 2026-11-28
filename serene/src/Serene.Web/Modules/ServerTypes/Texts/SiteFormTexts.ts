import { proxyTexts } from "@serenity-is/corelib";

namespace texts {
    export declare namespace Forms {
        export function asKey(): typeof Forms;
        export function asTry(): typeof Forms;
        namespace Membership {
            export function asKey(): typeof Membership;
            export function asTry(): typeof Membership;
            namespace Login {
                export function asKey(): typeof Login;
                export function asTry(): typeof Login;
                export const ForgotPassword: string;
                export const LoginToYourAccount: string;
                export const RememberMe: string;
                export const SignInButton: string;
                export const SignUpButton: string;
            }
            namespace SignUp {
                export function asKey(): typeof SignUp;
                export function asTry(): typeof SignUp;
                export const ActivateEmailButton: string;
                export const ActivateEmailExpires: string;
                export const ActivateEmailGreeting: string;
                export const ActivateEmailIgnore: string;
                export const ActivateEmailIntro: string;
                export const ActivateEmailSubject: string;
                export const ActivationCompleteMessage: string;
                export const ConfirmEmail: string;
                export const ConfirmPassword: string;
                export const DisplayName: string;
                export const Email: string;
                export const FormInfo: string;
                export const FormTitle: string;
                export const Password: string;
                export const SubmitButton: string;
                export const Success: string;
            }
        }
        export const SiteTitle: string;
    }
}

export const SiteFormTexts: typeof texts.Forms = /*#__PURE__*/ proxyTexts({}, 'Forms.', {
    Membership: {
        Login: {},
        SignUp: {}
    }
}) as any;