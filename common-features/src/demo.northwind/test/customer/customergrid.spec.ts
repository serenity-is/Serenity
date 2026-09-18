import { mockDynamicData, mockFetch, mockGridSize, unmockFetch } from "test-utils";
import { CustomerDialog } from "../../Modules/Customer/CustomerDialog";
import { CustomerGrid } from "../../Modules/Customer/CustomerGrid";
import { CustomerColumns, CustomerRow, CustomerService } from "../../Modules/ServerTypes/Demo";

beforeAll(() => {
    mockDynamicData();
    mockGridSize();
    mockFetch({ "*": () => ({ Entities: [], TotalCount: 0 }) });
});

afterAll(() => {
    unmockFetch();
});

afterEach(() => {
    vi.restoreAllMocks();
    document.body.innerHTML = "";
});

describe("CustomerGrid", () => {
    it("wires up columns, dialog, row and service", () => {
        const grid = new CustomerGrid({});
        expect(grid["getColumnsKey"]()).toBe(CustomerColumns.columnsKey);
        expect(grid["getDialogType"]()).toBe(CustomerDialog);
        expect(grid["getRowDefinition"]()).toBe(CustomerRow);
        expect(grid["getService"]()).toBe(CustomerService.baseUrl);
        grid.destroy();
    });
});
