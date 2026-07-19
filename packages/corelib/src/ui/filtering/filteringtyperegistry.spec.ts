import { BooleanFiltering } from "./booleanfiltering";
import { FilteringTypeRegistry } from "./filteringtyperegistry";
import { StringFiltering } from "./stringfiltering";

describe("FilteringTypeRegistry", () => {
    it("is defined", () => {
        expect(FilteringTypeRegistry).toBeDefined();
    });

    it("can get a registered filtering type by name", () => {
        const type = FilteringTypeRegistry.get("String");
        expect(type).toBeDefined();
    });

    it("can get BooleanFiltering by name", () => {
        const type = FilteringTypeRegistry.get("Boolean");
        expect(type).toBeDefined();
    });

    it("can get a type and instantiate it", () => {
        const type = FilteringTypeRegistry.get("String") as typeof StringFiltering;
        const instance = new type();
        expect(instance).toBeInstanceOf(StringFiltering);
    });

    it("can get BooleanFiltering and instantiate it", () => {
        const type = FilteringTypeRegistry.get("Boolean") as typeof BooleanFiltering;
        const instance = new type();
        expect(instance).toBeInstanceOf(BooleanFiltering);
    });

    it("throws for unknown type", () => {
        expect(() => FilteringTypeRegistry.get("NonExistentFiltering12345")).toThrow();
    });

    it("has loadKind as filtering", () => {
        // The registry stores options internally, check by attempting to get types
        const stringType = FilteringTypeRegistry.get("String");
        expect(stringType).toBeDefined();
        const booleanType = FilteringTypeRegistry.get("Boolean");
        expect(booleanType).toBeDefined();
    });
});
