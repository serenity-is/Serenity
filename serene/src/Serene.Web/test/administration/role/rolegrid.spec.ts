import { mockAdmin, mockDynamicData, mockFetch, mockGridSize, unmockFetch } from "test-utils";
import { afterEach, beforeAll, beforeEach, describe, expect, it, vi } from "vitest";
import { RoleDialog } from "../../../Modules/Administration/Role/RoleDialog";
import { RoleGrid } from "../../../Modules/Administration/Role/RoleGrid";
import { RoleColumns, RoleRow, RoleService } from "../../../Modules/ServerTypes/Administration";

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

describe("RoleGrid", () => {
    it("wires up columns, dialog, id, localText and service", async () => {
        const grid = new RoleGrid({});
        expect(grid["getColumnsKey"]()).toBe(RoleColumns.columnsKey);
        expect(grid["getDialogType"]()).toBe(RoleDialog);
        expect(grid["getIdProperty"]()).toBe(RoleRow.idProperty);
        expect(grid["getLocalTextPrefix"]()).toBe(RoleRow.localTextPrefix);
        expect(grid["getService"]()).toBe(RoleService.baseUrl);
        expect(grid["getDefaultSortBy"]()).toEqual([RoleRow.Fields.RoleName]);
        await new Promise(resolve => setTimeout(resolve, 10));
        grid.destroy();
    });
});
