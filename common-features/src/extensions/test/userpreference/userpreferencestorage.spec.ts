import { mockFetch, unmockFetch } from "test-utils";
import { afterEach, beforeEach, describe, expect, it } from "vitest";
import { UserPreferenceStorage } from "../../Modules/UserPreference/UserPreferenceStorage";

beforeEach(() => {
    mockFetch({
        "Extensions/UserPreference/Retrieve": info => {
            expect(info.data.PreferenceType).toBe("UserPreferenceStorage");
            return { Value: "stored-value" };
        },
        "Extensions/UserPreference/Update": info => {
            expect(info.data.PreferenceType).toBe("UserPreferenceStorage");
            return {};
        }
    });
});

afterEach(() => {
    unmockFetch();
});

describe("UserPreferenceStorage", () => {
    it("gets an item using the retrieve service", async () => {
        const storage = new UserPreferenceStorage();
        await expect(storage.getItem("my-key")).resolves.toBe("stored-value");
    });

    it("sets an item using the update service", async () => {
        const storage = new UserPreferenceStorage();
        await expect(storage.setItem("my-key", "my-value")).resolves.toEqual({});
    });
});
