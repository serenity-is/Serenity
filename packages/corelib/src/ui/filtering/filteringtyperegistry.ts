import { htmlEncode, isAssignableFrom, notifyError } from "../../base";
import { BaseTypeRegistry } from "../../types/basetyperegistry";
import { IFiltering } from "./ifiltering";

class FilteringTypeRegistryImpl extends BaseTypeRegistry<Function> {
    constructor() {
        super({
            loadKind: "filtering",
            defaultSuffix: "Filtering"
        });
    }

    protected override isMatchingType(type: any): boolean {
        return isAssignableFrom(IFiltering, type);
    }

    protected override loadError(key: string) {
        const message = `"${htmlEncode(key)}" filtering handler class not found!`;
        notifyError(message);
        throw new Error(message);
    }
}

export const FilteringTypeRegistry = new FilteringTypeRegistryImpl();