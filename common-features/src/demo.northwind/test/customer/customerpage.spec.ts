import * as corelib from "@serenity-is/corelib";
import { mockDynamicData, mockFetch, mockGridSize, unmockFetch } from "test-utils";
import { CustomerGrid } from "../../Modules/Customer/CustomerGrid";
import initPage from "../../Modules/Customer/CustomerPage";

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
});

describe("CustomerPage", () => {
    it("initializes the grid page", () => {
        const spy = vi.spyOn(corelib, "gridPageInit").mockReturnValue({} as any);
        initPage();
        expect(spy).toHaveBeenCalledWith(CustomerGrid);
    });
});
