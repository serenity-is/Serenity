import { mockRowLookup } from "test-utils";
import { RoleCheckEditor } from "../../../Modules/Administration/UserRole/RoleCheckEditor";
import { RoleRow } from "../../../Modules/ServerTypes/Administration";

beforeEach(() => {
    mockRowLookup(RoleRow, [{ RoleId: 1, RoleName: "Admin" }, { RoleId: 2, RoleName: "Editor" }] as any);
});

afterEach(() => {
    vi.restoreAllMocks();
    document.body.innerHTML = "";
});

describe("RoleCheckEditor", () => {
    it("builds tree items from the role lookup and has no buttons", async () => {
        const editor = new RoleCheckEditor({});
        expect(editor["getButtons"]()).toEqual([]);
        expect(editor["getTreeItems"]().map((x: any) => x.text)).toEqual(["Admin", "Editor"]);
        await new Promise(resolve => setTimeout(resolve, 10));
        editor.destroy();
    });

    it("filters items by search text", async () => {
        const editor = new RoleCheckEditor({});
        editor["searchText"] = "ADM";
        expect(editor["onViewFilter"]({ id: "1", text: "Admin" })).toBe(true);
        expect(editor["onViewFilter"]({ id: "2", text: "Editor" })).toBe(false);
        await new Promise(resolve => setTimeout(resolve, 10));
        editor.destroy();
    });

    it("reacts to quick search input", async () => {
        const editor = new RoleCheckEditor({});
        const input = editor.domNode.querySelector(".s-QuickSearchBar input") as HTMLInputElement;
        expect(input).toBeTruthy();
        input.value = "Admin";
        input.dispatchEvent(new Event("input", { bubbles: true }));
        await new Promise(resolve => setTimeout(resolve, 600));
        editor.destroy();
    });
});
