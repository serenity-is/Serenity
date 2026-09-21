import { describe, expect, it } from "vitest";
import { AccountPanelTitle } from "../../../Modules/Membership/Account/AccountPanelTitle";

describe("AccountPanelTitle", () => {
    it("renders the site title header", () => {
        expect(AccountPanelTitle()).toBeTruthy();
    });
});
