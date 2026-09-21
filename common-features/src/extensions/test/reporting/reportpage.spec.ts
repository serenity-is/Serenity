import { afterEach, describe, expect, it, vi } from "vitest";
import { ReportDialog } from "../../Modules/Reporting/ReportDialog";
import { ReportPage } from "../../Modules/Reporting/ReportPage";

function makePage() {
    const el = document.createElement("div");
    el.innerHTML = `
        <div class="s-QuickSearchBar"><input type="text" /></div>
        <ul class="report-list">
            <li class="report-item">Alpha</li>
            <li class="report-item">B&eacute;ta</li>
            <li>
                <ul>
                    <li class="report-item">Gamma</li>
                </ul>
            </li>
        </ul>
        <a class="report-link" data-key="Some.Report">Link</a>
    `;
    document.body.appendChild(el);
    return new ReportPage({ element: el });
}

afterEach(() => {
    document.body.innerHTML = "";
    vi.restoreAllMocks();
});

describe("ReportPage", () => {
    it("clears match flags for empty search", () => {
        const page = makePage();
        const items = page.domNode.querySelectorAll(".report-item");
        items.forEach(x => x.classList.add("non-match"));
        page["updateMatchFlags"]("");
        items.forEach(x => expect(x.classList.contains("non-match")).toBe(false));
        page.destroy();
    });

    it("marks non matching items", () => {
        const page = makePage();
        page["updateMatchFlags"]("Alpha");
        const items = Array.from(page.domNode.querySelectorAll(".report-item"));
        expect(items[0].classList.contains("non-match")).toBe(false);
        expect(items[1].classList.contains("non-match")).toBe(true);
        page.destroy();
    });

    it("matches without diacritics", () => {
        const page = makePage();
        page["updateMatchFlags"]("beta");
        const items = Array.from(page.domNode.querySelectorAll(".report-item"));
        expect(items[1].classList.contains("non-match")).toBe(false);
        page.destroy();
    });

    it("opens a report dialog on link click", () => {
        const page = makePage();
        const loadSpy = vi.spyOn(ReportDialog.prototype as any, "loadReport").mockImplementation(() => { });
        const openSpy = vi.spyOn(ReportDialog.prototype as any, "dialogOpen").mockImplementation(() => { });
        const link = page.domNode.querySelector(".report-link");
        const preventDefault = vi.fn();
        page["reportLinkClick"]({ preventDefault, target: link } as any);
        expect(preventDefault).toHaveBeenCalled();
        expect(loadSpy).toHaveBeenCalledWith("Some.Report");
        expect(openSpy).toHaveBeenCalled();
        page.destroy();
    });
});
