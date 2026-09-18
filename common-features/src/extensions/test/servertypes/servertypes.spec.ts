import * as Extensions from "../../Modules/ServerTypes/Extensions";
import * as Reporting from "../../Modules/ServerTypes/Reporting";
import * as Namespaces from "../../Modules/ServerTypes/Namespaces";
import * as RemoteDataKeys from "../../Modules/ServerTypes/RemoteDataKeys";
import * as Texts from "../../Modules/ServerTypes/Texts";
import { UserPreferenceRow } from "../../Modules/ServerTypes/Extensions/UserPreferenceRow";
import { UserPreferenceService } from "../../Modules/ServerTypes/Extensions/UserPreferenceService";
import { ChangePasswordForm } from "../../Modules/ServerTypes/Extensions/ChangePasswordForm";
import { ForgotPasswordForm } from "../../Modules/ServerTypes/Extensions/ForgotPasswordForm";
import { ResetPasswordForm } from "../../Modules/ServerTypes/Extensions/ResetPasswordForm";

describe("ServerTypes barrels", () => {
    it("loads all Extensions exports", () => {
        expect(Extensions).toBeTruthy();
        expect(Reporting).toBeTruthy();
        expect(Texts).toBeTruthy();
    });

    it("exposes namespaces", () => {
        expect(Namespaces.ExtensionsNS).toBe("Serenity.Extensions");
        expect(Namespaces.nsExtensions).toBe("Serenity.Extensions.");
        expect(Namespaces.ReportingNS).toBe("Serenity.Reporting");
        expect(Namespaces.nsReporting).toBe("Serenity.Reporting.");
    });

    it("exposes remote data keys", () => {
        expect(RemoteDataKeys.RemoteDataKeys.PasswordStrengthRules).toBe("PasswordStrengthRules");
    });

    it("exposes UserPreferenceService methods", () => {
        expect(UserPreferenceService.baseUrl).toBe("Extensions/UserPreference");
        expect(UserPreferenceService.Methods.Update).toBe("Extensions/UserPreference/Update");
        expect(UserPreferenceService.Methods.Retrieve).toBe("Extensions/UserPreference/Retrieve");
        expect(typeof (UserPreferenceService as any).Update).toBe("function");
        expect(typeof (UserPreferenceService as any).Retrieve).toBe("function");
    });

    it("exposes UserPreferenceRow static fields", () => {
        expect(UserPreferenceRow.idProperty).toBe("UserPreferenceId");
        expect(UserPreferenceRow.nameProperty).toBe("Name");
        expect(UserPreferenceRow.localTextPrefix).toBe("Common.UserPreference");
        expect(UserPreferenceRow.Fields).toBeTruthy();
    });

    it("registers form types", () => {
        const el = document.createElement("form");
        new ChangePasswordForm({ element: el } as any);
        new ForgotPasswordForm({ element: el } as any);
        new ResetPasswordForm({ element: el } as any);
        expect(ChangePasswordForm.formKey).toBe("Serenity.Extensions.ChangePasswordRequest");
        expect(ForgotPasswordForm.formKey).toBe("Serenity.Extensions.ForgotPasswordRequest");
        expect(ResetPasswordForm.formKey).toBe("Serenity.Extensions.ResetPasswordRequest");
    });
});
