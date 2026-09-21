import { mockDynamicData, mockFetch, mockGridSize, mockRowLookup, unmockFetch } from "test-utils";
import { afterAll, afterEach, beforeAll, describe, expect, it, vi } from "vitest";
import { CustomerOrderDialog } from "../../Modules/Customer/CustomerOrderDialog";
import { CustomerRow } from "../../Modules/ServerTypes/Demo";

beforeAll(() => {
    mockDynamicData();
    mockGridSize();
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

describe("CustomerOrderDialog", () => {
    it("makes the customer field read only", () => {
        const dialog = new CustomerOrderDialog({});
        (dialog as any).entity = {};
        (dialog as any).entityId = 1;
        expect(() => dialog["updateInterface"]()).not.toThrow();
        expect(dialog["form"].CustomerID.readOnly).toBe(true);
        dialog.destroy();
    });
});
