import { mockAdmin, mockDynamicData, mockFetch, mockGridSize, unmockFetch } from "test-utils";
import { afterEach, beforeAll, beforeEach, describe, expect, it, vi } from "vitest";
import { LanguageDialog } from "../../../Modules/Administration/Language/LanguageDialog";
import { LanguageGrid } from "../../../Modules/Administration/Language/LanguageGrid";
import { LanguageColumns, LanguageRow, LanguageService } from "../../../Modules/ServerTypes/Administration";

beforeAll(() => {
    mockDynamicData();
    mockAdmin();
    mockGridSize();
});

beforeEach(() => {
    mockFetch({ "*": () => ({ Entities: [], TotalCount: 0 }) });
});

afterEach(() => {
    unmockFetch();
    vi.restoreAllMocks();
    document.body.innerHTML = "";
});

describe("LanguageGrid", () => {
    it("wires up async, columns, dialog, id, localText and service", async () => {
        const grid = new LanguageGrid({});
        expect(grid["useAsync"]()).toBe(true);
        expect(grid["getColumnsKey"]()).toBe(LanguageColumns.columnsKey);
        expect(grid["getDialogType"]()).toBe(LanguageDialog);
        expect(grid["getIdProperty"]()).toBe(LanguageRow.idProperty);
        expect(grid["getLocalTextPrefix"]()).toBe(LanguageRow.localTextPrefix);
        expect(grid["getService"]()).toBe(LanguageService.baseUrl);
        expect(grid["getDefaultSortBy"]()).toEqual([LanguageRow.Fields.LanguageName]);
        await new Promise(resolve => setTimeout(resolve, 10));
        grid.destroy();
    });
});
