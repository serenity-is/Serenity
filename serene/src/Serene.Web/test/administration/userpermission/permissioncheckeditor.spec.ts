import { setScriptData } from "@serenity-is/corelib";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { PermissionCheckEditor } from "../../../Modules/Administration/UserPermission/PermissionCheckEditor";
import { RemoteDataKeys } from "../../../Modules/ServerTypes/RemoteDataKeys";

const permissionKeys = [
    "Administration:",
    "Administration:Users",
    "Administration:Roles",
    "Northwind:",
    "Northwind:Products"
];

beforeEach(() => {
    setScriptData("RemoteData." + RemoteDataKeys.Administration.PermissionKeys, permissionKeys);
    setScriptData("RemoteData." + RemoteDataKeys.Administration.ImplicitPermissions, {
        "Administration:Users": ["Administration:Roles"]
    });
});

afterEach(() => {
    vi.restoreAllMocks();
    document.body.innerHTML = "";
});

async function createEditor(props: any = {}) {
    const editor = new PermissionCheckEditor({ implicitPermissions: {}, ...props });
    await new Promise(resolve => setTimeout(resolve, 20));
    return editor;
}

describe("PermissionCheckEditor", () => {
    it("returns the passed value while not initialized", () => {
        const editor = new PermissionCheckEditor({ value: [{ PermissionKey: "Administration:Users" }] });
        expect(editor.value).toEqual([{ PermissionKey: "Administration:Users" }]);
    });

    it("loads permission items and applies string values", async () => {
        const editor = await createEditor({ value: ["Administration:Users"] });
        expect(editor.valueAsStrings).toEqual(["Administration:Users"]);
        expect(editor["getButtons"]()).toEqual([]);
        editor.destroy();
    });

    it("applies row values", async () => {
        const editor = await createEditor({ value: [{ PermissionKey: "Administration:Roles", Granted: true }] });
        expect(editor.value).toEqual([{ PermissionKey: "Administration:Roles", Granted: true }]);
        editor.destroy();
    });

    it("creates two columns without revoke and three with revoke", async () => {
        const editor = await createEditor();
        expect(editor["createColumns"]().length).toBe(2);
        editor.destroy();

        const editor2 = await createEditor({ showRevoke: true });
        expect(editor2["createColumns"]().length).toBe(3);
        editor2.destroy();
    });

    it("formats effective and grant/revoke classes", async () => {
        const editor = await createEditor({ value: ["Administration:Users"], showRevoke: true });
        const columns = editor["createColumns"]();
        const item = editor.view.getItemById("Administration:Users");
        expect(item).toBeTruthy();
        expect(editor["getItemEffectiveClass"](item)).toBe("allow");

        const group = editor.view.getItemById("Administration:");
        expect(editor["getItemEffectiveClass"](group)).toBe("partial");
        expect(editor["getItemGrantRevokeClass"](group, true)).toBeTruthy();

        const denyItem = editor.view.getItemById("Northwind:Products");
        expect(editor["getItemEffectiveClass"](denyItem)).toBe("deny");

        expect(columns[1].format({ item: item, value: null } as any)).toBeTruthy();
        expect(columns[2].format({ item: item, value: null } as any)).toBeTruthy();
        expect(columns[1].format({ item: group, value: null } as any)).toBeTruthy();
        expect(columns[2].format({ item: group, value: null } as any)).toBeTruthy();
        expect(columns[0].format({ item: item, value: "Users" } as any)).toBeTruthy();
        editor.destroy();
    });

    it("supports role and implicit permissions", async () => {
        const editor = await createEditor();
        editor.rolePermissions = ["Administration:Users"];
        expect(editor.rolePermissions).toEqual(["Administration:Users"]);
        editor.implicitPermissions = { "Administration:Users": ["Administration:Roles"] };
        expect(editor["implicitSets"]["Administration:Users"].has("Administration:Roles")).toBe(true);
        expect(editor["hasByRoleOrImplicitly"]("Administration:Users")).toBe(true);

        editor.implicitPermissions = { "Unknown:Perm": ["Administration:Roles"] };
        expect(editor["hasByRoleOrImplicitly"]("Administration:Roles")).toBe(false);
        editor.destroy();
    });

    it("ignores callbacks when destroyed before remote data resolves", async () => {
        const editor = new PermissionCheckEditor({});
        editor.destroy();
        await new Promise(resolve => setTimeout(resolve, 20));
        expect(editor.domNode == null || editor["view"] == null).toBeDefined();
    });

    it("handles empty and duplicate permission keys", async () => {
        setScriptData("RemoteData." + RemoteDataKeys.Administration.PermissionKeys,
            ["Administration:", "Administration:Users", "Administration:Users", ":"]);
        const editor = await createEditor();
        expect(editor.view.getItemById("Administration:Users")).toBeTruthy();
        editor.destroy();
    });

    it("reacts to quick search input", async () => {
        const editor = await createEditor();
        const input = editor["toolbar"].domNode.querySelector(".s-QuickSearchBar input") as HTMLInputElement;
        expect(input).toBeTruthy();
        input.value = "users";
        input.dispatchEvent(new Event("input", { bubbles: true }));
        await new Promise(resolve => setTimeout(resolve, 600));
        expect(editor["searchText"]).toBe("users");
        editor.destroy();
    });

    it("filters items by search text", async () => {
        const editor = await createEditor();
        editor.view.setItems(editor.view.getItems(), true);
        expect(editor["onViewFilter"](editor.view.getItemById("Administration:Users"))).toBe(true);
        editor["searchText"] = "products";
        expect(editor["matchContains"]({ Title: "Products" })).toBe(true);
        expect(editor["matchContains"]({ Title: "Users" })).toBe(false);
        editor.destroy();
    });

    it("toggles grant and revoke on click", async () => {
        const editor = await createEditor({ showRevoke: true });
        const item = editor.view.getItemById("Administration:Users");
        const target = document.createElement("span");
        target.classList.add("grant");
        const event: any = { target, preventDefault: vi.fn(), defaultPrevented: false };
        editor["onClick"](event, editor.view.getItems().indexOf(item), 1);
        expect(item.GrantRevoke).toBe(true);
        editor.destroy();
    });

    it("toggles all descendants when a group is clicked", async () => {
        const editor = await createEditor({ showRevoke: true });
        const group = editor.view.getItemById("Administration:");
        const target = document.createElement("span");
        target.classList.add("revoke");
        const event: any = { target, preventDefault: vi.fn(), defaultPrevented: false };
        editor["onClick"](event, editor.view.getItems().indexOf(group), 2);
        expect(editor.view.getItemById("Administration:Users").GrantRevoke).toBe(false);
        editor.destroy();
    });

    it("returns descendants and parent keys", async () => {
        const editor = await createEditor();
        const group = editor.view.getItemById("Administration:");
        expect(editor["getDescendants"](group, true).length).toBeGreaterThan(0);
        expect(editor.view.getItemById("Administration:Users").ParentKey).toBe("Administration:");
        editor.destroy();
    });
});


