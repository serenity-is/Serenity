import * as corelib from "@serenity-is/corelib";
import { mockDynamicData, mockFetch, mockGridSize, unmockFetch } from "test-utils";
import initPage from "../../Modules/Region/RegionPage";
import { RegionGrid } from "../../Modules/Region/RegionGrid";

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

describe("RegionPage", () => {
    it("initializes the grid page", () => {
        const spy = vi.spyOn(corelib, "gridPageInit").mockReturnValue({} as any);
        initPage();
        expect(spy).toHaveBeenCalledWith(RegionGrid);
    });
});
