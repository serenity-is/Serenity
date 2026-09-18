import * as corelib from "@serenity-is/corelib";
import { mockDynamicData, mockFetch, mockGridSize, unmockFetch } from "test-utils";
import initPage from "../../Modules/Category/CategoryPage";
import { CategoryGrid } from "../../Modules/Category/CategoryGrid";

beforeAll(() => {
    mockDynamicData();
    mockGridSize();
});

beforeEach(() => {
    mockFetch({ "*": () => ({ Entities: [], TotalCount: 0 }) });
});

afterEach(() => {
    unmockFetch();
    vi.restoreAllMocks();
});

describe("CategoryPage", () => {
    it("initializes the grid page", () => {
        const spy = vi.spyOn(corelib, "gridPageInit").mockReturnValue({} as any);
        initPage();
        expect(spy).toHaveBeenCalledWith(CategoryGrid);
    });
});
