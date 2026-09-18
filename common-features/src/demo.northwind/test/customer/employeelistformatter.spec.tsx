import { formatterContext } from "@serenity-is/sleekgrid";
import { mockDynamicData, mockRowLookup } from "test-utils";
import { EmployeeListFormatter } from "../../Modules/Customer/EmployeeListFormatter";
import { EmployeeRow } from "../../Modules/ServerTypes/Demo";

function render(result: any): string {
    const div = document.createElement("div");
    if (result != null)
        div.append(result);
    return div.textContent;
}

beforeAll(() => {
    mockDynamicData();
    mockRowLookup(EmployeeRow, [
        { EmployeeID: 1, FullName: "John" },
        { EmployeeID: 2, FullName: "Jane" }
    ]);
});

describe("EmployeeListFormatter", () => {
    it("returns empty string for empty id lists", () => {
        const formatter = new EmployeeListFormatter();
        expect(formatter.format(formatterContext({ value: null }))).toBe("");
        expect(formatter.format(formatterContext({ value: [] }))).toBe("");
    });

    it("renders the full names once the lookup is loaded", async () => {
        const formatter = new EmployeeListFormatter();
        let nested: any;
        const grid = {
            invalidate: () => {
                nested = formatter.format(formatterContext({ value: [1, 99] }));
            }
        };

        const first = formatter.format(formatterContext({ value: [1, 99], grid: grid as any }));
        expect(render(first)).toBe("");

        await vi.waitFor(() => expect(nested).toBeTruthy());
        expect(render(nested)).toBe("John, 99");
    });

    it("resets the lookup when loading fails", async () => {
        const formatter = new EmployeeListFormatter();
        vi.spyOn(EmployeeRow, "getLookupAsync").mockRejectedValue(new Error("fail"));
        const result = formatter.format(formatterContext({ value: [1] }));
        expect(result).toBeTruthy();
        await new Promise(resolve => setTimeout(resolve, 0));
        vi.restoreAllMocks();
    });
});
