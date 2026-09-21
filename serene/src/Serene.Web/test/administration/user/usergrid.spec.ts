import { mockAdmin, mockDynamicData, mockFetch, mockGridSize, mockRowLookup, unmockFetch } from "test-utils";
import { afterAll, afterEach, beforeAll, beforeEach, describe, expect, it, vi } from "vitest";
import { UserDialog } from "../../../Modules/Administration/User/UserDialog";
import { UserGrid } from "../../../Modules/Administration/User/UserGrid";
import { RoleRow, UserColumns, UserRow, UserService } from "../../../Modules/ServerTypes/Administration";

beforeAll(() => {
    mockDynamicData();
    mockAdmin();
    mockGridSize();
    mockFetch({ "*": () => ({ Entities: [], TotalCount: 0 }) });
});

afterAll(() => {
    unmockFetch();
});

beforeEach(() => {
    mockRowLookup(RoleRow, [{ RoleId: 1, RoleName: "Admin" }] as any);
});

afterEach(() => {
    vi.restoreAllMocks();
    document.body.innerHTML = "";
});

describe("UserGrid", () => {
    it("wires up columns, dialog, id, isActive, localText and service", async () => {
        const grid = new UserGrid({});
        expect(grid["getColumnsKey"]()).toBe(UserColumns.columnsKey);
        expect(grid["getDialogType"]()).toBe(UserDialog);
        expect(grid["getIdProperty"]()).toBe(UserRow.idProperty);
        expect(grid["getIsActiveProperty"]()).toBe(UserRow.isActiveProperty);
        expect(grid["getLocalTextPrefix"]()).toBe(UserRow.localTextPrefix);
        expect(grid["getService"]()).toBe(UserService.baseUrl);
        expect(grid["getDefaultSortBy"]()).toEqual([UserRow.Fields.Username]);
        expect(grid["createIncludeDeletedButton"]()).toBeUndefined();
        await new Promise(resolve => setTimeout(resolve, 20));
        grid.destroy();
    });

    it("formats the roles column with role names", async () => {
        const grid = new UserGrid({});
        const columns = grid["createColumns"]();
        const roles = columns.find((x: any) => x.field == UserRow.Fields.Roles);
        expect(roles).toBeTruthy();
        await new Promise(resolve => setTimeout(resolve, 20));
        const result = roles.format({ value: [1], escape: (x: string) => x, item: {} } as any);
        expect(result).toContain("Admin");
        grid.destroy();
    });

    it("exposes deprecated lookup accessors", async () => {
        mockRowLookup(UserRow, [{ UserId: 1, Username: "admin" }] as any);
        expect(UserRow.getLookup().items.length).toBe(1);
        const lookup = await UserRow.getLookupAsync();
        expect(lookup.items.length).toBe(1);
    });
});
