import { AccountPanelTitle } from "../../Modules/Membership/AccountPanelTitle";

describe("AccountPanelTitle", () => {
    it("renders the site title panel", () => {
        const el = AccountPanelTitle() as HTMLElement;
        const container = document.createElement("div");
        container.appendChild(el);
        const h2 = container.querySelector("h2");
        expect(h2).toBeTruthy();
        expect(h2.classList.contains("text-center")).toBe(true);
        expect(h2.querySelector("img.s-site-logo-img")).toBeTruthy();
    });
});
