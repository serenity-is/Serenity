import { afterEach, describe, expect, it, vi } from "vitest";
import { SampleInfo, SampleSourceLink, getSamplePagePath } from "../Modules/sample-info";

function resetDom() {
    document.head.innerHTML = `<meta name="sample-page-path" content="~/Serenity.Demo.BasicSamples/esm/Modules/Grids/Test/TestPage.js">` +
        `<meta name="repository-blob-url" content="https://repo/">`;
    document.body.innerHTML = `<section class="content"></section>`;
}

afterEach(() => {
    vi.restoreAllMocks();
    document.body.innerHTML = "";
});

describe("getSamplePagePath", () => {
    it("normalizes the sample page path", () => {
        resetDom();
        const path = getSamplePagePath();
        expect(path).toBe("Modules/Grids/Test/TestPage");
        expect(getSamplePagePath()).toBe(path);
    });

    it("returns an empty string when there is no meta tag", () => {
        document.head.innerHTML = "";
        document.body.innerHTML = "";
        expect(typeof getSamplePagePath()).toBe("string");
    });
});

describe("SampleInfo", () => {
    it("renders a content header and sample info block with a given title", () => {
        resetDom();
        const markup = SampleInfo({ contentTitle: "My Sample", children: "Some content", sources: [".css"] });
        expect(markup).toBeTruthy();
        const section = document.querySelector("section.content");
        expect(section.querySelector("h1").textContent).toBe("My Sample");
        expect(section.querySelector(".s-sample-info")).toBeTruthy();
        expect(section.querySelector(".s-sample-sources").textContent).toContain("TestPage.tsx");
        expect(section.querySelector(".s-sample-sources").textContent).toContain("TestPage.css");
    });

    it("falls back to the document title without the last segment", () => {
        resetDom();
        document.title = "Grids - Test Page - Site";
        SampleInfo({ children: "Content" });
        expect(document.querySelector("section.content h1").textContent).toBe("Grids - Test Page");
    });

    it("does not add a header when there is no title", () => {
        resetDom();
        document.title = "NoSeparator";
        SampleInfo({ children: "Content" });
        const section = document.querySelector("section.content");
        expect(section.querySelector("h1")).toBeNull();
        expect(section.querySelector(".s-sample-info")).toBeTruthy();
    });
});

describe("SampleSourceLink", () => {
    it("builds an absolute href for paths starting with slash", () => {
        resetDom();
        const link = SampleSourceLink({ path: "/Modules/Test/File.ts" });
        const div = document.createElement("div");
        div.append(link);
        const anchor = div.querySelector("a.s-sample-source-link") as HTMLAnchorElement;
        expect(anchor.getAttribute("href")).toBe("https://repo/Modules/Test/File.ts");
        expect(anchor.textContent).toBe("File.ts");
    });

    it("builds a relative href for plain paths", () => {
        resetDom();
        const link = SampleSourceLink({ path: "Other.ts" });
        const div = document.createElement("div");
        div.append(link);
        const anchor = div.querySelector("a.s-sample-source-link") as HTMLAnchorElement;
        expect(anchor.getAttribute("href")).toContain("common-features/src/demo.basicsamples/Modules/Grids/Test/Other.ts");
    });
});
