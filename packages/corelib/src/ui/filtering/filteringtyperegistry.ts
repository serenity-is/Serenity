import { htmlEncode, isAssignableFrom, notifyError } from "../../base";
import { BaseTypeRegistry } from "../../types/basetyperegistry";
import { IFiltering } from "./ifiltering";

class FilteringTypeRegistryImpl extends BaseTypeRegistry<Function> {
    /**
     * Creates the filtering type registry.
     */
    constructor() {
        super({
            loadKind: "filtering",
            defaultSuffix: "Filtering"
        });
    }

    /**
     * Whether a type is a matching filtering handler.
     * @param type - The type to check.
     * @returns True when assignable from IFiltering.
     */
    protected override isMatchingType(type: any): boolean {
        return isAssignableFrom(IFiltering, type);
    }

    /**
     * Returns the error for a missing filtering handler.
     * @param key - The missing key.
     * @returns The error.
     */
    protected override loadError(key: string) {
        const message = `The filtering handler class "${key}" was not found!`;
        notifyError(message);
        throw new Error(message);
    }
}

/**
 * Registry for filtering handler types, resolved by filtering type key.
 */
export const FilteringTypeRegistry = new FilteringTypeRegistryImpl();