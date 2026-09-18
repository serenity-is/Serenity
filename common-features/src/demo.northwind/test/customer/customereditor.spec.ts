import { mockDynamicData, mockFetch, mockRowLookup, unmockFetch } from "test-utils";
import { CustomerEditor } from "../../Modules/Customer/CustomerEditor";
import { CustomerRow } from "../../Modules/ServerTypes/Demo";

beforeAll(() => {
    mockDynamicData();
    mockRowLookup(CustomerRow, [{ CustomerID: "ALFKI", CompanyName: "Alfreds" }]);
    mockFetch({ "*": () => ({ Entities: [], TotalCount: 0 }) });
});

afterAll(() => {
    unmockFetch();
});

afterEach(() => {
    vi.restoreAllMocks();
    document.body.innerHTML = "";
});

describe("CustomerEditor", () => {
    it("uses the Northwind.Customer lookup key", () => {
        const editor = new CustomerEditor({});
        expect(editor["getLookupKey"]()).toBe("Northwind.Customer");
        editor.destroy();
    });

    it("appends the customer id to the item text", () => {
        const editor = new CustomerEditor({});
        const text = editor["getItemText"]({ CustomerID: "ALFKI", CompanyName: "Alfreds" }, { textField: "CompanyName" } as any);
        expect(text).toBe("Alfreds [ALFKI]");
        editor.destroy();
    });
});
